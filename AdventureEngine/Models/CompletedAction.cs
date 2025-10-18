namespace AdventureEngine.Models;

/// <summary>
/// Tracks which actions have been completed in a game save
/// </summary>
public class CompletedAction
{
    public int Id { get; set; }
    public int GameSaveId { get; set; }
    public int RoomActionId { get; set; }
    public DateTime CompletedAt { get; set; }

    // Relationships
    public GameSave GameSave { get; set; } = null!;
    public RoomAction RoomAction { get; set; } = null!;
}
