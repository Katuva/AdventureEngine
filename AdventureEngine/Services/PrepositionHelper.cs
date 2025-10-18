namespace AdventureEngine.Services;

/// <summary>
/// Helper class for identifying and working with prepositions in commands
/// </summary>
public static class PrepositionHelper
{
    /// <summary>
    /// Common prepositions used in text adventure commands
    /// </summary>
    private static readonly HashSet<string> Prepositions = new(StringComparer.OrdinalIgnoreCase)
    {
        // Location prepositions
        "in", "into", "inside",
        "on", "onto", "upon",
        "under", "underneath", "beneath",
        "behind",
        "beside", "near", "next to",

        // Instrumental prepositions
        "with", "using",

        // Directional prepositions
        "to", "toward", "towards",
        "from",
        "at",

        // Other useful prepositions
        "through",
        "over",
        "across",
        "around",
        "about"
    };

    /// <summary>
    /// Articles to strip from input
    /// </summary>
    private static readonly HashSet<string> Articles = new(StringComparer.OrdinalIgnoreCase)
    {
        "the", "a", "an", "some"
    };

    /// <summary>
    /// Conjunctions for multi-object commands
    /// </summary>
    private static readonly HashSet<string> Conjunctions = new(StringComparer.OrdinalIgnoreCase)
    {
        "and", "then"
    };

    /// <summary>
    /// Keywords for multi-object operations
    /// </summary>
    private static readonly HashSet<string> MultiObjectKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "all", "everything", "each", "every"
    };

    /// <summary>
    /// Pronouns for context references
    /// </summary>
    private static readonly HashSet<string> Pronouns = new(StringComparer.OrdinalIgnoreCase)
    {
        "it", "that", "this", "them", "these", "those"
    };

    /// <summary>
    /// Check if a word is a preposition
    /// </summary>
    public static bool IsPreposition(string word)
    {
        return Prepositions.Contains(word);
    }

    /// <summary>
    /// Check if a word is an article
    /// </summary>
    public static bool IsArticle(string word)
    {
        return Articles.Contains(word);
    }

    /// <summary>
    /// Check if a word is a conjunction
    /// </summary>
    public static bool IsConjunction(string word)
    {
        return Conjunctions.Contains(word);
    }

    /// <summary>
    /// Check if a word should be ignored during parsing (article or filler word)
    /// </summary>
    public static bool ShouldIgnore(string word)
    {
        return IsArticle(word);
    }

    /// <summary>
    /// Check if a word is a multi-object keyword (all, everything)
    /// </summary>
    public static bool IsMultiObjectKeyword(string word)
    {
        return MultiObjectKeywords.Contains(word);
    }

    /// <summary>
    /// Check if a word is a pronoun (it, that, them)
    /// </summary>
    public static bool IsPronoun(string word)
    {
        return Pronouns.Contains(word);
    }

    /// <summary>
    /// Normalize a preposition to its canonical form
    /// </summary>
    public static string NormalizePreposition(string preposition)
    {
        return preposition.ToLower() switch
        {
            "into" => "in",
            "inside" => "in",
            "onto" => "on",
            "upon" => "on",
            "underneath" => "under",
            "beneath" => "under",
            "using" => "with",
            "toward" => "to",
            "towards" => "to",
            "next to" => "beside",
            _ => preposition.ToLower()
        };
    }

    /// <summary>
    /// Get all recognized prepositions
    /// </summary>
    public static IEnumerable<string> GetAllPrepositions() => Prepositions;

    /// <summary>
    /// Remove articles from a list of words
    /// </summary>
    public static List<string> StripArticles(IEnumerable<string> words)
    {
        return words.Where(w => !IsArticle(w)).ToList();
    }

    /// <summary>
    /// Split text by conjunctions and return separate phrases
    /// Example: "lamp and sword" -> ["lamp", "sword"]
    /// </summary>
    public static List<string> SplitByConjunction(string text)
    {
        var parts = new List<string>();
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var currentPhrase = new List<string>();

        foreach (var word in words)
        {
            if (IsConjunction(word))
            {
                // Save current phrase if not empty
                if (currentPhrase.Count > 0)
                {
                    parts.Add(string.Join(" ", currentPhrase));
                    currentPhrase.Clear();
                }
            }
            else if (!IsArticle(word))
            {
                currentPhrase.Add(word);
            }
        }

        // Add final phrase
        if (currentPhrase.Count > 0)
        {
            parts.Add(string.Join(" ", currentPhrase));
        }

        return parts;
    }
}
