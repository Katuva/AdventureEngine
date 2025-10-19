namespace AdventureEngine.Models;

/// <summary>
/// Represents an item that can be found in rooms or carried by the player
/// </summary>
public class Item
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    // Item properties
    public bool IsCollectable { get; set; } = true;
    public bool IsQuestItem { get; set; }
    public int Weight { get; set; }
    public string? UseMessage { get; set; }

    // Healing properties
    public int HealingAmount { get; set; }
    public int MaxUses { get; set; } = 1; // Default: single use (0 = unlimited)
    public string? EmptyDescription { get; set; } // Description when item is empty
    public bool DisappearsWhenEmpty { get; set; } // If true, item is removed from inventory when uses reach 0

    // Location tracking
    public int? RoomId { get; set; }
    public Room? Room { get; set; }

    // Relationships
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
}
