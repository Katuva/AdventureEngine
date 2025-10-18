namespace AdventureEngine.Models;

/// <summary>
/// Represents a saved game state
/// </summary>
public class GameSave
{
    public int Id { get; set; }
    public required string SlotName { get; set; }
    public DateTime SavedAt { get; set; }
    public int CurrentRoomId { get; set; }
    public Room CurrentRoom { get; set; } = null!;

    // Game statistics
    public int TurnCount { get; set; }
    public int Score { get; set; }
    public int Health { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsPlayerDead { get; set; }

    // Player inventory for this save
    public ICollection<InventoryItem> Inventory { get; set; } = new List<InventoryItem>();

    // Completed actions tracking
    public ICollection<CompletedAction> CompletedActions { get; set; } = new List<CompletedAction>();
}
