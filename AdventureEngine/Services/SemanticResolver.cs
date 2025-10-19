using AdventureEngine.Data;
using AdventureEngine.Models;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Services;

/// <summary>
/// Resolves parsed text to actual game entities using vocabulary and semantic matching
/// Phase 2: Vocabulary-driven object resolution
/// </summary>
public class SemanticResolver
{
    private readonly AdventureDbContext _context;

    public SemanticResolver(AdventureDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Normalize a verb using vocabulary synonyms
    /// Example: "get" -> "take", "grab" -> "take"
    /// </summary>
    public async Task<string> NormalizeVerbAsync(string verb)
    {
        var vocab = await _context.Vocabularies
            .FirstOrDefaultAsync(v => v.Word == verb.ToLower() && v.WordType == WordTypes.Verb);

        if (vocab?.CanonicalForm != null)
        {
            return vocab.CanonicalForm;
        }

        return verb.ToLower();
    }

    /// <summary>
    /// Find an item in the context (room or inventory) that matches the description
    /// Supports adjective matching for disambiguation
    /// </summary>
    /// <summary>
    /// Find an item in the context (room or inventory) that matches the description
    /// Supports adjective matching, fuzzy matching, and returns all matches for ambiguity resolution
    /// </summary>
    public async Task<List<Item>> ResolveItemsAsync(
        string description,
        GameStateManager gameState,
        bool includeInventory = true,
        bool includeRoom = true)
    {
        var words = description.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var adjectives = new List<string>();
        var noun = words.Last(); // Last word is usually the noun

        // Everything before the last word could be adjectives
        if (words.Length > 1)
        {
            adjectives.AddRange(words[..^1]);
        }

        // Normalize adjectives and noun using vocabulary
        var normalizedAdjectives = new List<string>();
        foreach (var adj in adjectives)
        {
            var normalized = await NormalizeAdjectiveAsync(adj);
            normalizedAdjectives.Add(normalized);
        }

        var normalizedNoun = await NormalizeNounAsync(noun);

        // Get available items
        var candidates = await GetAvailableItemsAsync(gameState, includeInventory, includeRoom);

        // Try exact match with adjectives first
        if (normalizedAdjectives.Count > 0)
        {
            var exactMatches = await FindItemsByAdjectivesAsync(candidates, normalizedAdjectives, normalizedNoun);
            if (exactMatches.Count > 0)
            {
                return exactMatches;
            }
        }

        // Try direct name match
        var nounMatches = candidates.Where(i =>
            i.Name.ToLower().Contains(normalizedNoun) ||
            i.Name.ToLower().Contains(noun)).ToList();

        if (nounMatches.Count > 0)
        {
            return nounMatches;
        }

        // Check reverse synonyms
        var reverseMatches = new List<Item>();
        foreach (var candidate in candidates)
        {
            var itemNameLower = candidate.Name.ToLower();
            var vocab = await _context.Vocabularies
                .FirstOrDefaultAsync(v => v.Word == itemNameLower &&
                                         v.WordType == WordTypes.Noun &&
                                         v.CanonicalForm == normalizedNoun);

            if (vocab != null)
            {
                reverseMatches.Add(candidate);
            }
        }

        if (reverseMatches.Count > 0)
        {
            return reverseMatches;
        }

        // Try fuzzy matching as last resort
        // Check against both the original noun AND normalized noun
        var fuzzyMatches = new List<Item>();
        foreach (var candidate in candidates)
        {
            // Check if fuzzy match with normalized noun
            if (FuzzyMatcher.IsSimilar(normalizedNoun, candidate.Name, maxDistance: 2))
            {
                fuzzyMatches.Add(candidate);
                continue;
            }

            // Also check against the original noun (handles typos like "lmap" -> "Lamp")
            if (FuzzyMatcher.IsSimilar(noun, candidate.Name, maxDistance: 2))
            {
                fuzzyMatches.Add(candidate);
                continue;
            }

            // Check against vocabulary canonical forms too
            var itemNameLower = candidate.Name.ToLower();
            var vocabEntry = await _context.Vocabularies
                .FirstOrDefaultAsync(v => v.Word == itemNameLower && v.WordType == WordTypes.Noun);

            if (vocabEntry?.CanonicalForm != null)
            {
                // Check if typo is close to the canonical form
                if (FuzzyMatcher.IsSimilar(noun, vocabEntry.CanonicalForm, maxDistance: 2))
                {
                    fuzzyMatches.Add(candidate);
                }
            }
        }

        return fuzzyMatches;
    }

    /// <summary>
    /// Resolve to a single item (backward compatibility)
    /// Returns null if multiple matches (ambiguous)
    /// </summary>
    public async Task<Item?> ResolveItemAsync(
        string description,
        GameStateManager gameState,
        bool includeInventory = true,
        bool includeRoom = true)
    {
        var matches = await ResolveItemsAsync(description, gameState, includeInventory, includeRoom);

        if (matches.Count == 1)
        {
            return matches[0];
        }

        // If multiple matches, try auto-disambiguation
        if (matches.Count > 1)
        {
            var resolver = new AmbiguityResolver();
            return AmbiguityResolver.AutoDisambiguate(matches);
        }

        return null;
    }

    /// <summary>
    /// Resolve an examinable object by name/keywords with fuzzy matching support
    /// Respects IsHidden property and reveal state
    /// </summary>
    public async Task<ExaminableObject?> ResolveExaminableObjectAsync(
        string description,
        int roomId,
        GameStateManager? gameState = null)
    {
        var lowerDesc = description.ToLower();

        // Get only visible examinable objects (if gameState provided)
        List<ExaminableObject> candidateObjects;
        if (gameState != null)
        {
            candidateObjects = await gameState.GetVisibleExaminableObjectsAsync(roomId);
        }
        else
        {
            // Fallback: get all objects (for backward compatibility)
            candidateObjects = await _context.ExaminableObjects
                .Where(eo => eo.RoomId == roomId)
                .ToListAsync();
        }

        // Try exact/contains match first
        var examinable = candidateObjects
            .FirstOrDefault(eo => eo.Name.ToLower().Contains(lowerDesc) ||
                                 (eo.Keywords != null && eo.Keywords.ToLower().Contains(lowerDesc)));

        if (examinable != null)
        {
            return examinable;
        }

        // Try fuzzy matching as fallback
        foreach (var obj in candidateObjects)
        {
            // Check fuzzy match against name
            if (FuzzyMatcher.IsSimilar(lowerDesc, obj.Name, maxDistance: 2))
            {
                return obj;
            }

            // Check fuzzy match against keywords
            if (obj.Keywords != null)
            {
                var keywords = obj.Keywords.ToLower().Split(',', StringSplitOptions.TrimEntries);
                foreach (var keyword in keywords)
                {
                    if (FuzzyMatcher.IsSimilar(lowerDesc, keyword, maxDistance: 2))
                    {
                        return obj;
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Normalize an adjective using vocabulary
    /// Example: "gold" -> "golden"
    /// </summary>
    private async Task<string> NormalizeAdjectiveAsync(string adjective)
    {
        var vocab = await _context.Vocabularies
            .FirstOrDefaultAsync(v => v.Word == adjective.ToLower() && v.WordType == WordTypes.Adjective);

        if (vocab?.CanonicalForm != null)
        {
            return vocab.CanonicalForm;
        }

        return adjective.ToLower();
    }

    /// <summary>
    /// Normalize a noun using vocabulary
    /// Example: "lantern" -> "lamp", "blade" -> "sword"
    /// </summary>
    private async Task<string> NormalizeNounAsync(string noun)
    {
        var vocab = await _context.Vocabularies
            .FirstOrDefaultAsync(v => v.Word == noun.ToLower() && v.WordType == WordTypes.Noun);

        if (vocab?.CanonicalForm != null)
        {
            return vocab.CanonicalForm;
        }

        return noun.ToLower();
    }

    /// <summary>
    /// Get list of items available to the player in current context
    /// </summary>
    private async Task<List<Item>> GetAvailableItemsAsync(
        GameStateManager gameState,
        bool includeInventory,
        bool includeRoom)
    {
        var items = new List<Item>();

        if (includeInventory)
        {
            var inventoryItems = await _context.InventoryItems
                .Include(ii => ii.Item)
                .Where(ii => ii.GameSaveId == gameState.CurrentSaveId)
                .Select(ii => ii.Item)
                .ToListAsync();

            items.AddRange(inventoryItems);
        }

        if (includeRoom)
        {
            var room = await gameState.GetCurrentRoomAsync();
            if (room != null)
            {
                // Get items in this save's inventory to exclude them
                var itemsInInventory = await _context.InventoryItems
                    .Where(ii => ii.GameSaveId == gameState.CurrentSaveId)
                    .Select(ii => ii.ItemId)
                    .ToListAsync();

                // Get original room items
                var roomItems = await _context.Items
                    .Where(i => i.RoomId == room.Id && !itemsInInventory.Contains(i.Id))
                    .ToListAsync();

                items.AddRange(roomItems);

                // Get placed items
                var placedItems = await _context.PlacedItems
                    .Include(pi => pi.Item)
                    .Where(pi => pi.GameSaveId == gameState.CurrentSaveId && pi.RoomId == room.Id)
                    .Select(pi => pi.Item)
                    .ToListAsync();

                items.AddRange(placedItems);
            }
        }

        return items.Distinct().ToList();
    }

    /// <summary>
    /// Find all items that match all specified adjectives
    /// </summary>
    private async Task<List<Item>> FindItemsByAdjectivesAsync(
        List<Item> candidates,
        List<string> adjectives,
        string noun)
    {
        var matches = new List<Item>();

        foreach (var item in candidates)
        {
            // Check if item name contains the noun
            if (!item.Name.ToLower().Contains(noun))
            {
                continue;
            }

            // Get item's adjectives from database
            var itemAdjectives = await _context.ItemAdjectives
                .Where(ia => ia.ItemId == item.Id)
                .Select(ia => ia.Adjective.ToLower())
                .ToListAsync();

            // Check if all provided adjectives match
            var allMatch = adjectives.All(adj => itemAdjectives.Contains(adj));

            if (allMatch)
            {
                matches.Add(item);
            }
        }

        return matches;
    }
}
