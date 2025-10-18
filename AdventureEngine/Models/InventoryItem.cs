namespace AdventureEngine.Models;

/// <summary>
/// Represents an item in a player's inventory (many-to-many relationship)
/// </summary>
public class InventoryItem
{
    public int Id { get; set; }
    public int GameSaveId { get; set; }
    public int ItemId { get; set; }
    public DateTime PickedUpAt { get; set; }

    // Relationships
    public GameSave GameSave { get; set; } = null!;
    public Item Item { get; set; } = null!;
}
