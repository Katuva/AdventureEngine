namespace AdventureEngine.Models;

/// <summary>
/// Tracks the open/closed and locked/unlocked state of containers per save game
/// </summary>
public class ContainerState
{
    public int Id { get; set; }
    public int GameSaveId { get; set; }
    public GameSave GameSave { get; set; } = null!;
    public int ContainerId { get; set; }
    public Container Container { get; set; } = null!;

    /// <summary>
    /// Whether the container is currently open
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// Whether the container is currently locked
    /// </summary>
    public bool IsLocked { get; set; }

    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}
