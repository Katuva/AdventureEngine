namespace AdventureEngine.Models;

/// <summary>
/// Represents a location/area in the game world
/// </summary>
public class Room
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    // Navigation connections to other rooms
    public int? NorthRoomId { get; set; }
    public int? SouthRoomId { get; set; }
    public int? EastRoomId { get; set; }
    public int? WestRoomId { get; set; }
    public int? UpRoomId { get; set; }
    public int? DownRoomId { get; set; }

    // Room properties
    public bool IsStartingRoom { get; set; }
    public bool IsWinningRoom { get; set; }
    public bool IsDeadlyRoom { get; set; }
    public int DamageAmount { get; set; }
    public string? DeathMessage { get; set; }
    public string? WinMessage { get; set; }
    public bool IsDark { get; set; }  // If true, requires light source to see
    public int? LightSourceItemId { get; set; }  // Item that provides light (e.g., lantern)
    public Item? LightSourceItem { get; set; }
    public int? ProtectionItemId { get; set; }  // Item that protects from damage
    public Item? ProtectionItem { get; set; }

    /// <summary>
    /// Required state for the protection item (e.g., "lit" for lantern, null for items that just need to be carried)
    /// </summary>
    public string? RequiredItemState { get; set; }

    // Navigation relationships
    public Room? NorthRoom { get; set; }
    public Room? SouthRoom { get; set; }
    public Room? EastRoom { get; set; }
    public Room? WestRoom { get; set; }
    public Room? UpRoom { get; set; }
    public Room? DownRoom { get; set; }

    // Collections
    public ICollection<Item> Items { get; set; } = new List<Item>();
    public ICollection<RoomAction> Actions { get; set; } = new List<RoomAction>();
}
