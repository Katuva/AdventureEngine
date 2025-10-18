using AdventureEngine.Models;

namespace AdventureEngine.Services;

/// <summary>
/// Handles ambiguous references when multiple items match player input
/// Phase 3: Ambiguity resolution
/// </summary>
public class AmbiguityResolver
{
    /// <summary>
    /// Result of attempting to resolve an ambiguous reference
    /// </summary>
    public class AmbiguityResult
    {
        public bool IsAmbiguous { get; set; }
        public Item? ResolvedItem { get; set; }
        public List<Item> Candidates { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Try to resolve ambiguity among multiple items
    /// Returns a result indicating if disambiguation is needed
    /// </summary>
    public AmbiguityResult ResolveItems(List<Item> candidates, string originalInput)
    {
        if (candidates.Count == 0)
        {
            return new AmbiguityResult
            {
                IsAmbiguous = false,
                ErrorMessage = $"There is no '{originalInput}' here."
            };
        }

        if (candidates.Count == 1)
        {
            return new AmbiguityResult
            {
                IsAmbiguous = false,
                ResolvedItem = candidates[0]
            };
        }

        // Multiple matches - ambiguous
        return new AmbiguityResult
        {
            IsAmbiguous = true,
            Candidates = candidates,
            ErrorMessage = BuildAmbiguityMessage(candidates)
        };
    }

    /// <summary>
    /// Build a user-friendly ambiguity message
    /// </summary>
    private string BuildAmbiguityMessage(List<Item> candidates)
    {
        var message = "Which do you mean:";
        for (int i = 0; i < candidates.Count; i++)
        {
            message += $"\n  {i + 1}. {candidates[i].Name}";
        }
        message += "\n(Please be more specific, e.g., use an adjective)";
        return message;
    }

    /// <summary>
    /// Try to auto-disambiguate using item priority or uniqueness
    /// </summary>
    public Item? AutoDisambiguate(List<Item> candidates)
    {
        if (candidates.Count == 0)
        {
            return null;
        }

        if (candidates.Count == 1)
        {
            return candidates[0];
        }

        // Prefer quest items
        var questItems = candidates.Where(i => i.IsQuestItem).ToList();
        if (questItems.Count == 1)
        {
            return questItems[0];
        }

        // Prefer collectable items over non-collectable
        var collectableItems = candidates.Where(i => i.IsCollectable).ToList();
        if (collectableItems.Count == 1)
        {
            return collectableItems[0];
        }

        // Can't auto-disambiguate
        return null;
    }
}
