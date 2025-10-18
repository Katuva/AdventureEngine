namespace AdventureEngine.Models;

/// <summary>
/// Tracks which examinable object interactions have been completed in a game save
/// </summary>
public class CompletedExaminableInteraction
{
    public int Id { get; set; }
    public int GameSaveId { get; set; }
    public int ExaminableObjectId { get; set; }
    public DateTime CompletedAt { get; set; }

    // Relationships
    public GameSave GameSave { get; set; } = null!;
    public ExaminableObject ExaminableObject { get; set; } = null!;
}
