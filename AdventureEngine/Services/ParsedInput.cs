namespace AdventureEngine.Services;

/// <summary>
/// Represents a parsed user command with structured components
/// </summary>
public class ParsedInput
{
    /// <summary>
    /// The primary verb/command (e.g., "take", "use", "put")
    /// </summary>
    public string Verb { get; set; } = string.Empty;

    /// <summary>
    /// Direct objects - things being acted upon (e.g., ["lamp", "sword"] from "take lamp and sword")
    /// </summary>
    public List<string> DirectObjects { get; set; } = new();

    /// <summary>
    /// Preposition connecting objects (e.g., "in", "on", "with", "from")
    /// </summary>
    public string? Preposition { get; set; }

    /// <summary>
    /// Indirect object - target of preposition (e.g., "box" from "put lamp in box")
    /// </summary>
    public string? IndirectObject { get; set; }

    /// <summary>
    /// Original raw input for fallback
    /// </summary>
    public string RawInput { get; set; } = string.Empty;

    /// <summary>
    /// All words after the verb (for backward compatibility)
    /// </summary>
    public string[] Args => GetArgs();

    private string[] GetArgs()
    {
        var args = new List<string>();

        // Add direct objects
        args.AddRange(DirectObjects);

        // Add preposition and indirect object if present
        if (!string.IsNullOrEmpty(Preposition) && !string.IsNullOrEmpty(IndirectObject))
        {
            args.Add(Preposition);
            args.Add(IndirectObject);
        }

        return args.ToArray();
    }

    /// <summary>
    /// Check if this is a simple command (verb only, or verb + direct objects)
    /// </summary>
    public bool IsSimple => string.IsNullOrEmpty(Preposition);

    /// <summary>
    /// Check if this command has multiple direct objects (conjunction)
    /// </summary>
    public bool HasMultipleObjects => DirectObjects.Count > 1;

    /// <summary>
    /// True if the command uses "all" or "everything"
    /// </summary>
    public bool IsMultiObjectCommand { get; set; }

    /// <summary>
    /// True if the command uses pronouns (it, that, them)
    /// </summary>
    public bool UsesPronoun { get; set; }

    /// <summary>
    /// Creates a simple parsed input (verb + args, no structure)
    /// </summary>
    public static ParsedInput CreateSimple(string verb, params string[] args)
    {
        return new ParsedInput
        {
            Verb = verb,
            DirectObjects = args.ToList(),
            RawInput = $"{verb} {string.Join(" ", args)}"
        };
    }
}
