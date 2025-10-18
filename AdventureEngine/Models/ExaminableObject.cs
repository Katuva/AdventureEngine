namespace AdventureEngine.Models;

/// <summary>
/// Represents an object in a room that can be examined for details, secrets, or interactions
/// </summary>
public class ExaminableObject
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    /// <summary>
    /// The name/keyword used to examine this object (e.g., "statue", "book", "trapdoor outline")
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The text shown when examining this object
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// If true, this object is only visible after certain conditions are met
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// The action that must be completed to reveal this object (if hidden)
    /// </summary>
    public int? RevealedByActionId { get; set; }
    public RoomAction? RevealedByAction { get; set; }

    /// <summary>
    /// Keywords/aliases that can be used to examine this object
    /// </summary>
    public string? Keywords { get; set; }

    /// <summary>
    /// The item required to interact with this object (e.g., key for a lock)
    /// </summary>
    public int? RequiredItemId { get; set; }
    public Item? RequiredItem { get; set; }

    /// <summary>
    /// The room that gets unlocked when using the required item on this object
    /// </summary>
    public int? UnlocksRoomId { get; set; }
    public Room? UnlocksRoom { get; set; }

    /// <summary>
    /// Message shown when successfully using the required item on this object
    /// </summary>
    public string? SuccessMessage { get; set; }

    /// <summary>
    /// Message shown when trying to interact without the required item
    /// </summary>
    public string? FailureMessage { get; set; }
}
