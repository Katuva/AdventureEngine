namespace AdventureEngine.Models;

/// <summary>
/// Tracks how many times an item has been used in a specific save
/// Used for consumable/healing items with limited uses
/// </summary>
public class ItemUsage
{
    public int Id { get; set; }
    public int GameSaveId { get; set; }
    public GameSave GameSave { get; set; } = null!;
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    /// <summary>
    /// Number of times this item has been used in this save
    /// </summary>
    public int UsesCount { get; set; }

    /// <summary>
    /// When this item was last used
    /// </summary>
    public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
}
