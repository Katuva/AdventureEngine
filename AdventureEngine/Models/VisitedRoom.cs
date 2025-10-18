namespace AdventureEngine.Models;

/// <summary>
/// Tracks which rooms have been visited by each save game
/// </summary>
public class VisitedRoom
{
    public int Id { get; set; }
    public int GameSaveId { get; set; }
    public GameSave GameSave { get; set; } = null!;
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public DateTime FirstVisitedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastVisitedAt { get; set; } = DateTime.UtcNow;
    public int VisitCount { get; set; } = 1;
}
