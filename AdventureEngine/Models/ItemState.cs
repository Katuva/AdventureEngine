namespace AdventureEngine.Models;

/// <summary>
/// Tracks per-save state for items (lit/unlit, open/closed, etc.)
/// </summary>
public class ItemState
{
    public int Id { get; set; }
    public int GameSaveId { get; set; }
    public GameSave GameSave { get; set; } = null!;
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    /// <summary>
    /// Current state of the item (e.g., "lit", "unlit", "open", "closed")
    /// </summary>
    public string State { get; set; } = "default";

    /// <summary>
    /// When this state was last changed
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Common item states
/// </summary>
public static class ItemStates
{
    public const string Default = "default";
    public const string Lit = "lit";
    public const string Unlit = "unlit";
    public const string Open = "open";
    public const string Closed = "closed";
    public const string Active = "active";
    public const string Inactive = "inactive";
    public const string Empty = "empty";
}
