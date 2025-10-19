namespace AdventureEngine.Models;

/// <summary>
/// Tracks how many times an examinable object has been used/activated in a specific save
/// Used for healing fountains, wells, or other limited-use interactable objects
/// </summary>
public class ExaminableObjectUsage
{
    public int Id { get; set; }
    public int GameSaveId { get; set; }
    public GameSave GameSave { get; set; } = null!;
    public int ExaminableObjectId { get; set; }
    public ExaminableObject ExaminableObject { get; set; } = null!;

    /// <summary>
    /// Number of times this object has been used in this save
    /// </summary>
    public int UsesCount { get; set; }

    /// <summary>
    /// When this object was last used
    /// </summary>
    public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
}
