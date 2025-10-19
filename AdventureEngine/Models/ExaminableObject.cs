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
    /// Used for command matching (case-insensitive)
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Display name shown to player (e.g., "Chandelier", "Hidden Safe")
    /// If null, uses Name. Use this for proper capitalization in output.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// The text shown when examining this object (detailed description)
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Short description shown in room's "look" command (if ShowInRoomDescription is true)
    /// If null, uses Description. Keep this brief (one sentence).
    /// </summary>
    public string? LookDescription { get; set; }

    /// <summary>
    /// If true, this object is only visible after certain conditions are met
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// If true, this object appears in the room description when using "look" command
    /// If false, the object can only be discovered by specifically examining it
    /// Note: Only applies to visible objects (IsHidden=false or already revealed)
    /// </summary>
    public bool ShowInRoomDescription { get; set; }

    /// <summary>
    /// The action that must be completed to reveal this object (if hidden)
    /// </summary>
    public int? RevealedByActionId { get; set; }
    public RoomAction? RevealedByAction { get; set; }

    /// <summary>
    /// The examinable object that must be examined to reveal this object (if hidden)
    /// </summary>
    public int? RevealedByExaminableId { get; set; }
    public ExaminableObject? RevealedByExaminable { get; set; }

    /// <summary>
    /// The item that must be picked up to reveal this object (if hidden)
    /// </summary>
    public int? RevealedByItemId { get; set; }
    public Item? RevealedByItem { get; set; }

    /// <summary>
    /// Message shown when this object is revealed (optional)
    /// </summary>
    public string? RevealMessage { get; set; }

    /// <summary>
    /// If true, shows the RevealMessage when revealed (if player is in same room)
    /// If false, object is revealed silently even if player is in same room
    /// Default: true
    /// </summary>
    public bool ShowRevealMessage { get; set; } = true;

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
    /// The direction that gets unlocked (north, south, east, west, up, down)
    /// </summary>
    public string? UnlockDirection { get; set; }

    /// <summary>
    /// Message shown when successfully using the required item on this object
    /// </summary>
    public string? SuccessMessage { get; set; }

    /// <summary>
    /// Message shown when trying to interact without the required item
    /// </summary>
    public string? FailureMessage { get; set; }

    /// <summary>
    /// If true, this object can be activated/pressed/toggled (e.g., switch, lever, button)
    /// </summary>
    public bool IsActivatable { get; set; }

    /// <summary>
    /// If true, this object can only be activated once per save
    /// </summary>
    public bool IsOneTimeUse { get; set; }

    /// <summary>
    /// Message shown when successfully activating this object
    /// If null, uses default: "You activate the [name] and you hear a click, but you can't tell if anything happened"
    /// </summary>
    public string? ActivationMessage { get; set; }

    /// <summary>
    /// The examinable object that gets revealed when this object is activated (can be in any room)
    /// </summary>
    public int? RevealsExaminableId { get; set; }
    public ExaminableObject? RevealsExaminable { get; set; }
}
