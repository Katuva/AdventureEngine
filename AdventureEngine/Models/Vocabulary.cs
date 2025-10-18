namespace AdventureEngine.Models;

/// <summary>
/// Represents a word in the game's vocabulary with its type and meaning
/// </summary>
public class Vocabulary
{
    public int Id { get; set; }

    /// <summary>
    /// The word itself (lowercase)
    /// </summary>
    public required string Word { get; set; }

    /// <summary>
    /// Type of word (verb, noun, adjective, etc.)
    /// </summary>
    public required string WordType { get; set; }

    /// <summary>
    /// Normalized/canonical form if this is a synonym
    /// Example: "get" -> "take", "rusty" -> "rust"
    /// </summary>
    public string? CanonicalForm { get; set; }

    /// <summary>
    /// Category for grouping related words
    /// Example: "movement", "combat", "color", "material"
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Optional description/notes about this word
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// Word types supported by the vocabulary system
/// </summary>
public static class WordTypes
{
    public const string Verb = "verb";
    public const string Noun = "noun";
    public const string Adjective = "adjective";
    public const string Preposition = "preposition";
    public const string Article = "article";
    public const string Conjunction = "conjunction";
    public const string Direction = "direction";
}
