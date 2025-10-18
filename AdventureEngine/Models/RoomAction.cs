namespace AdventureEngine.Models;

/// <summary>
/// Represents special actions that can be performed in specific rooms
/// </summary>
public class RoomAction
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public required string ActionName { get; set; }
    public required string Description { get; set; }

    // Action requirements
    public int? RequiredItemId { get; set; }
    public Item? RequiredItem { get; set; }

    // Action effects
    public string? SuccessMessage { get; set; }
    public string? FailureMessage { get; set; }
    public int? UnlocksRoomId { get; set; }
    public Room? UnlocksRoom { get; set; }
    public bool IsRepeatable { get; set; } = true;

    // Relationships
    public Room Room { get; set; } = null!;
}
