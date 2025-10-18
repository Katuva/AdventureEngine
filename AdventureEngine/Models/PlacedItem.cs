namespace AdventureEngine.Models;

/// <summary>
/// Represents an item that has been placed/dropped in a room by a specific save game
/// </summary>
public class PlacedItem
{
    public int Id { get; set; }
    public int GameSaveId { get; set; }
    public GameSave GameSave { get; set; } = null!;
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public DateTime PlacedAt { get; set; }
}
