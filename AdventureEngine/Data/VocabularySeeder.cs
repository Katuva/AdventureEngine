using AdventureEngine.Models;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Data;

/// <summary>
/// Seeds the database with initial vocabulary for natural language parsing
/// </summary>
public class VocabularySeeder(AdventureDbContext context)
{
    public async Task SeedAsync()
    {
        // Check if already seeded
        var existingCount = await context.Vocabularies.CountAsync();
        if (existingCount > 0)
        {
            return; // Vocabulary already loaded
        }

        var vocabularies = new List<Vocabulary>();

        // === VERBS ===

        // Movement verbs
        vocabularies.Add(new Vocabulary { Word = "go", WordType = WordTypes.Verb, Category = "movement" });
        vocabularies.Add(new Vocabulary { Word = "walk", WordType = WordTypes.Verb, Category = "movement", CanonicalForm = "go" });
        vocabularies.Add(new Vocabulary { Word = "move", WordType = WordTypes.Verb, Category = "movement", CanonicalForm = "go" });
        vocabularies.Add(new Vocabulary { Word = "travel", WordType = WordTypes.Verb, Category = "movement", CanonicalForm = "go" });

        // Taking/getting verbs
        vocabularies.Add(new Vocabulary { Word = "take", WordType = WordTypes.Verb, Category = "acquisition" });
        vocabularies.Add(new Vocabulary { Word = "get", WordType = WordTypes.Verb, Category = "acquisition", CanonicalForm = "take" });
        vocabularies.Add(new Vocabulary { Word = "grab", WordType = WordTypes.Verb, Category = "acquisition", CanonicalForm = "take" });
        vocabularies.Add(new Vocabulary { Word = "pick", WordType = WordTypes.Verb, Category = "acquisition", CanonicalForm = "take" });
        vocabularies.Add(new Vocabulary { Word = "acquire", WordType = WordTypes.Verb, Category = "acquisition", CanonicalForm = "take" });
        vocabularies.Add(new Vocabulary { Word = "obtain", WordType = WordTypes.Verb, Category = "acquisition", CanonicalForm = "take" });

        // Dropping/placing verbs
        vocabularies.Add(new Vocabulary { Word = "drop", WordType = WordTypes.Verb, Category = "placement" });
        vocabularies.Add(new Vocabulary { Word = "place", WordType = WordTypes.Verb, Category = "placement", CanonicalForm = "drop" });
        vocabularies.Add(new Vocabulary { Word = "put", WordType = WordTypes.Verb, Category = "placement", CanonicalForm = "drop" });
        vocabularies.Add(new Vocabulary { Word = "set", WordType = WordTypes.Verb, Category = "placement", CanonicalForm = "drop" });
        vocabularies.Add(new Vocabulary { Word = "leave", WordType = WordTypes.Verb, Category = "placement", CanonicalForm = "drop" });

        // Examination verbs
        vocabularies.Add(new Vocabulary { Word = "look", WordType = WordTypes.Verb, Category = "examination" });
        vocabularies.Add(new Vocabulary { Word = "examine", WordType = WordTypes.Verb, Category = "examination" });
        vocabularies.Add(new Vocabulary { Word = "inspect", WordType = WordTypes.Verb, Category = "examination", CanonicalForm = "examine" });
        vocabularies.Add(new Vocabulary { Word = "check", WordType = WordTypes.Verb, Category = "examination", CanonicalForm = "examine" });
        vocabularies.Add(new Vocabulary { Word = "view", WordType = WordTypes.Verb, Category = "examination", CanonicalForm = "look" });
        vocabularies.Add(new Vocabulary { Word = "observe", WordType = WordTypes.Verb, Category = "examination", CanonicalForm = "examine" });
        vocabularies.Add(new Vocabulary { Word = "study", WordType = WordTypes.Verb, Category = "examination", CanonicalForm = "examine" });

        // Using/activating verbs
        vocabularies.Add(new Vocabulary { Word = "use", WordType = WordTypes.Verb, Category = "interaction" });
        vocabularies.Add(new Vocabulary { Word = "activate", WordType = WordTypes.Verb, Category = "interaction", CanonicalForm = "use" });
        vocabularies.Add(new Vocabulary { Word = "apply", WordType = WordTypes.Verb, Category = "interaction", CanonicalForm = "use" });
        vocabularies.Add(new Vocabulary { Word = "employ", WordType = WordTypes.Verb, Category = "interaction", CanonicalForm = "use" });

        // Inventory/status verbs
        vocabularies.Add(new Vocabulary { Word = "inventory", WordType = WordTypes.Verb, Category = "status" });
        vocabularies.Add(new Vocabulary { Word = "items", WordType = WordTypes.Verb, Category = "status", CanonicalForm = "inventory" });
        vocabularies.Add(new Vocabulary { Word = "carrying", WordType = WordTypes.Verb, Category = "status", CanonicalForm = "inventory" });

        // System verbs
        vocabularies.Add(new Vocabulary { Word = "quit", WordType = WordTypes.Verb, Category = "system" });
        vocabularies.Add(new Vocabulary { Word = "exit", WordType = WordTypes.Verb, Category = "system", CanonicalForm = "quit" });
        vocabularies.Add(new Vocabulary { Word = "help", WordType = WordTypes.Verb, Category = "system" });
        vocabularies.Add(new Vocabulary { Word = "commands", WordType = WordTypes.Verb, Category = "system", CanonicalForm = "help" });

        // === ADJECTIVES ===

        // Colors
        vocabularies.Add(new Vocabulary { Word = "golden", WordType = WordTypes.Adjective, Category = "color" });
        vocabularies.Add(new Vocabulary { Word = "gold", WordType = WordTypes.Adjective, Category = "color", CanonicalForm = "golden" });
        vocabularies.Add(new Vocabulary { Word = "rusty", WordType = WordTypes.Adjective, Category = "color" });
        vocabularies.Add(new Vocabulary { Word = "rust", WordType = WordTypes.Adjective, Category = "color", CanonicalForm = "rusty" });
        vocabularies.Add(new Vocabulary { Word = "brass", WordType = WordTypes.Adjective, Category = "material" });
        vocabularies.Add(new Vocabulary { Word = "silver", WordType = WordTypes.Adjective, Category = "color" });
        vocabularies.Add(new Vocabulary { Word = "bronze", WordType = WordTypes.Adjective, Category = "material" });
        vocabularies.Add(new Vocabulary { Word = "wooden", WordType = WordTypes.Adjective, Category = "material" });
        vocabularies.Add(new Vocabulary { Word = "wood", WordType = WordTypes.Adjective, Category = "material", CanonicalForm = "wooden" });
        vocabularies.Add(new Vocabulary { Word = "stone", WordType = WordTypes.Adjective, Category = "material" });
        vocabularies.Add(new Vocabulary { Word = "iron", WordType = WordTypes.Adjective, Category = "material" });
        vocabularies.Add(new Vocabulary { Word = "steel", WordType = WordTypes.Adjective, Category = "material" });

        // Size/age
        vocabularies.Add(new Vocabulary { Word = "old", WordType = WordTypes.Adjective, Category = "age" });
        vocabularies.Add(new Vocabulary { Word = "ancient", WordType = WordTypes.Adjective, Category = "age" });
        vocabularies.Add(new Vocabulary { Word = "new", WordType = WordTypes.Adjective, Category = "age" });
        vocabularies.Add(new Vocabulary { Word = "small", WordType = WordTypes.Adjective, Category = "size" });
        vocabularies.Add(new Vocabulary { Word = "large", WordType = WordTypes.Adjective, Category = "size" });
        vocabularies.Add(new Vocabulary { Word = "big", WordType = WordTypes.Adjective, Category = "size", CanonicalForm = "large" });
        vocabularies.Add(new Vocabulary { Word = "tiny", WordType = WordTypes.Adjective, Category = "size", CanonicalForm = "small" });
        vocabularies.Add(new Vocabulary { Word = "huge", WordType = WordTypes.Adjective, Category = "size", CanonicalForm = "large" });

        // Quality/condition
        vocabularies.Add(new Vocabulary { Word = "ornate", WordType = WordTypes.Adjective, Category = "quality" });
        vocabularies.Add(new Vocabulary { Word = "simple", WordType = WordTypes.Adjective, Category = "quality" });
        vocabularies.Add(new Vocabulary { Word = "heavy", WordType = WordTypes.Adjective, Category = "quality" });
        vocabularies.Add(new Vocabulary { Word = "light", WordType = WordTypes.Adjective, Category = "quality" });
        vocabularies.Add(new Vocabulary { Word = "dark", WordType = WordTypes.Adjective, Category = "quality" });
        vocabularies.Add(new Vocabulary { Word = "bright", WordType = WordTypes.Adjective, Category = "quality" });

        // === NOUNS (Common Objects) ===
        vocabularies.Add(new Vocabulary { Word = "key", WordType = WordTypes.Noun, Category = "tool" });
        vocabularies.Add(new Vocabulary { Word = "lamp", WordType = WordTypes.Noun, Category = "light" });
        vocabularies.Add(new Vocabulary { Word = "lantern", WordType = WordTypes.Noun, Category = "light", CanonicalForm = "lamp" });
        vocabularies.Add(new Vocabulary { Word = "light", WordType = WordTypes.Noun, Category = "light", CanonicalForm = "lamp" });
        vocabularies.Add(new Vocabulary { Word = "book", WordType = WordTypes.Noun, Category = "readable" });
        vocabularies.Add(new Vocabulary { Word = "tome", WordType = WordTypes.Noun, Category = "readable", CanonicalForm = "book" });
        vocabularies.Add(new Vocabulary { Word = "sword", WordType = WordTypes.Noun, Category = "weapon" });
        vocabularies.Add(new Vocabulary { Word = "blade", WordType = WordTypes.Noun, Category = "weapon", CanonicalForm = "sword" });
        vocabularies.Add(new Vocabulary { Word = "box", WordType = WordTypes.Noun, Category = "container" });
        vocabularies.Add(new Vocabulary { Word = "chest", WordType = WordTypes.Noun, Category = "container" });
        vocabularies.Add(new Vocabulary { Word = "statue", WordType = WordTypes.Noun, Category = "decoration" });
        vocabularies.Add(new Vocabulary { Word = "door", WordType = WordTypes.Noun, Category = "barrier" });
        vocabularies.Add(new Vocabulary { Word = "bookshelf", WordType = WordTypes.Noun, Category = "furniture" });
        vocabularies.Add(new Vocabulary { Word = "shelf", WordType = WordTypes.Noun, Category = "furniture" });

        // === DIRECTIONS ===
        vocabularies.Add(new Vocabulary { Word = "north", WordType = WordTypes.Direction, Category = "direction" });
        vocabularies.Add(new Vocabulary { Word = "south", WordType = WordTypes.Direction, Category = "direction" });
        vocabularies.Add(new Vocabulary { Word = "east", WordType = WordTypes.Direction, Category = "direction" });
        vocabularies.Add(new Vocabulary { Word = "west", WordType = WordTypes.Direction, Category = "direction" });
        vocabularies.Add(new Vocabulary { Word = "up", WordType = WordTypes.Direction, Category = "direction" });
        vocabularies.Add(new Vocabulary { Word = "down", WordType = WordTypes.Direction, Category = "direction" });
        vocabularies.Add(new Vocabulary { Word = "n", WordType = WordTypes.Direction, Category = "direction", CanonicalForm = "north" });
        vocabularies.Add(new Vocabulary { Word = "s", WordType = WordTypes.Direction, Category = "direction", CanonicalForm = "south" });
        vocabularies.Add(new Vocabulary { Word = "e", WordType = WordTypes.Direction, Category = "direction", CanonicalForm = "east" });
        vocabularies.Add(new Vocabulary { Word = "w", WordType = WordTypes.Direction, Category = "direction", CanonicalForm = "west" });
        vocabularies.Add(new Vocabulary { Word = "u", WordType = WordTypes.Direction, Category = "direction", CanonicalForm = "up" });
        vocabularies.Add(new Vocabulary { Word = "d", WordType = WordTypes.Direction, Category = "direction", CanonicalForm = "down" });

        context.Vocabularies.AddRange(vocabularies);
        await context.SaveChangesAsync();
    }
}
