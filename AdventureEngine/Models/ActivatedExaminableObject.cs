namespace AdventureEngine.Models;

/// <summary>
/// Tracks which activatable examinable objects have been activated for each game save
/// </summary>
public class ActivatedExaminableObject
{
    public int Id { get; set; }
    public int GameSaveId { get; set; }
    public GameSave GameSave { get; set; } = null!;
    public int ExaminableObjectId { get; set; }
    public ExaminableObject ExaminableObject { get; set; } = null!;
    public DateTime ActivatedAt { get; set; } = DateTime.UtcNow;
}
