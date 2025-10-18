namespace AdventureEngine.Models;

/// <summary>
/// Tracks the player's conversation context for pronoun resolution and smart defaults
/// Phase 3: Context-aware parsing
/// </summary>
public class PlayerContext
{
    public int Id { get; set; }

    /// <summary>
    /// The game save this context belongs to
    /// </summary>
    public int GameSaveId { get; set; }
    public GameSave GameSave { get; set; } = null!;

    /// <summary>
    /// Last item mentioned or interacted with (for "it", "that" references)
    /// </summary>
    public int? LastMentionedItemId { get; set; }
    public Item? LastMentionedItem { get; set; }

    /// <summary>
    /// Last examinable object examined (for pronoun references)
    /// </summary>
    public int? LastExaminedObjectId { get; set; }
    public ExaminableObject? LastExaminedObject { get; set; }

    /// <summary>
    /// Last room the player was in (for "go back" functionality)
    /// </summary>
    public int? LastRoomId { get; set; }
    public Room? LastRoom { get; set; }

    /// <summary>
    /// When this context was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
