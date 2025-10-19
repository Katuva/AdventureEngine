namespace AdventureEngine.Models;

/// <summary>
/// Tracks items that have been permanently removed from the game world (e.g., consumables that disappeared when empty)
/// </summary>
public class RemovedItem
{
    public int Id { get; set; }
    public int GameSaveId { get; set; }
    public GameSave GameSave { get; set; } = null!;
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public DateTime RemovedAt { get; set; } = DateTime.UtcNow;
}
