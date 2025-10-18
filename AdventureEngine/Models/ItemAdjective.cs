namespace AdventureEngine.Models;

/// <summary>
/// Links items to their adjectives for better semantic matching
/// Example: "Golden Key" has adjective "golden"
/// </summary>
public class ItemAdjective
{
    public int Id { get; set; }

    /// <summary>
    /// The item this adjective describes
    /// </summary>
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    /// <summary>
    /// The adjective word (lowercase)
    /// Example: "golden", "rusty", "brass"
    /// </summary>
    public required string Adjective { get; set; }

    /// <summary>
    /// Priority for disambiguation (higher = more distinctive)
    /// Example: "golden" might have higher priority than "old"
    /// </summary>
    public int Priority { get; set; } = 1;
}
