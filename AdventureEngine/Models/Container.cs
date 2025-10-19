namespace AdventureEngine.Models;

/// <summary>
/// Represents a container object (chest, box, cabinet, etc.) that can hold items
/// </summary>
public class Container
{
    public int Id { get; set; }
    public int? RoomId { get; set; }
    public Room? Room { get; set; }

    /// <summary>
    /// The name/keyword used to reference this container (e.g., "chest", "box")
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Display name shown to player (e.g., "Ornate Chest", "Wooden Box")
    /// If null, uses Name. Use this for proper capitalization in output.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Description shown when examining the container
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Keywords/aliases that can be used to reference this container
    /// </summary>
    public string? Keywords { get; set; }

    /// <summary>
    /// Description shown when container is open and has items
    /// </summary>
    public string? OpenDescription { get; set; }

    /// <summary>
    /// Description shown when container is open but empty
    /// </summary>
    public string? EmptyDescription { get; set; }

    /// <summary>
    /// If true, this container is initially open
    /// </summary>
    public bool StartsOpen { get; set; }

    /// <summary>
    /// If true, this container can be locked/unlocked
    /// </summary>
    public bool IsLockable { get; set; }

    /// <summary>
    /// If true, this container starts in a locked state
    /// </summary>
    public bool StartsLocked { get; set; }

    /// <summary>
    /// The item required to unlock this container (e.g., a key)
    /// </summary>
    public int? KeyItemId { get; set; }
    public Item? KeyItem { get; set; }

    /// <summary>
    /// Message shown when successfully unlocking
    /// </summary>
    public string? UnlockMessage { get; set; }

    /// <summary>
    /// Message shown when trying to open while locked
    /// </summary>
    public string? LockedMessage { get; set; }

    /// <summary>
    /// If true, container is visible in room descriptions
    /// </summary>
    public bool ShowInRoomDescription { get; set; } = true;

    /// <summary>
    /// If true, this container starts hidden and must be revealed
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// The examinable object that reveals this container when examined
    /// </summary>
    public int? RevealedByExaminableId { get; set; }
    public ExaminableObject? RevealedByExaminable { get; set; }

    /// <summary>
    /// Message shown when this container is revealed
    /// </summary>
    public string? RevealMessage { get; set; }

    /// <summary>
    /// Items contained in this container
    /// </summary>
    public ICollection<ContainerItem> ContainerItems { get; set; } = new List<ContainerItem>();
}
