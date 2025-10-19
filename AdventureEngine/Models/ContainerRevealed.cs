namespace AdventureEngine.Models;

/// <summary>
/// Tracks which containers have been revealed in each save game
/// </summary>
public class ContainerRevealed
{
    public int Id { get; set; }
    public int GameSaveId { get; set; }
    public GameSave GameSave { get; set; } = null!;
    public int ContainerId { get; set; }
    public Container Container { get; set; } = null!;
    public DateTime RevealedAt { get; set; }
}
