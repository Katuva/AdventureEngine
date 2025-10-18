namespace AdventureEngine.Models;

/// <summary>
/// Represents a conditional room description that changes based on game state
/// </summary>
public class RoomDescription
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    /// <summary>
    /// The description text to show when conditions are met
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Priority - higher priority descriptions are checked first (default: 0)
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// Condition type (e.g., "item_state", "has_item", "completed_action", "default")
    /// </summary>
    public required string ConditionType { get; set; }

    /// <summary>
    /// Item ID required for item-based conditions
    /// </summary>
    public int? RequiredItemId { get; set; }
    public Item? RequiredItem { get; set; }

    /// <summary>
    /// Required item state (e.g., "lit" for lantern)
    /// </summary>
    public string? RequiredItemState { get; set; }

    /// <summary>
    /// Required action ID for action-based conditions
    /// </summary>
    public int? RequiredActionId { get; set; }
    public RoomAction? RequiredAction { get; set; }

    /// <summary>
    /// Whether the action must be completed (true) or NOT completed (false)
    /// </summary>
    public bool ActionMustBeCompleted { get; set; } = true;

    /// <summary>
    /// Whether the item must be in inventory (true) or NOT in inventory (false)
    /// </summary>
    public bool ItemMustBeOwned { get; set; } = true;
}

/// <summary>
/// Condition types for room descriptions
/// </summary>
public static class DescriptionConditionTypes
{
    /// <summary>
    /// Default description - always matches (lowest priority)
    /// </summary>
    public const string Default = "default";

    /// <summary>
    /// Requires player to have item in specific state
    /// </summary>
    public const string ItemState = "item_state";

    /// <summary>
    /// Requires player to have (or not have) an item
    /// </summary>
    public const string HasItem = "has_item";

    /// <summary>
    /// Requires an action to be completed (or not completed)
    /// </summary>
    public const string CompletedAction = "completed_action";

    /// <summary>
    /// Always true - used for immediate transitions
    /// </summary>
    public const string Always = "always";
}
