namespace AdventureEngine.Models;

/// <summary>
/// Tracks which examinable objects have been revealed for each game save
/// </summary>
public class RevealedExaminableObject
{
    public int Id { get; set; }
    public int GameSaveId { get; set; }
    public GameSave GameSave { get; set; } = null!;
    public int ExaminableObjectId { get; set; }
    public ExaminableObject ExaminableObject { get; set; } = null!;
    public DateTime RevealedAt { get; set; } = DateTime.UtcNow;
}
