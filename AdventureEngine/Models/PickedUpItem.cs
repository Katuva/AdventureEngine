namespace AdventureEngine.Models;

/// <summary>
/// Tracks items that have been picked up at least once in this save.
/// Once picked up, items only appear where placed, not in their original room.
/// </summary>
public class PickedUpItem
{
    public int Id { get; set; }
    public int GameSaveId { get; set; }
    public GameSave GameSave { get; set; } = null!;
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public DateTime FirstPickedUpAt { get; set; } = DateTime.UtcNow;
}
