using AdventureEngine.Data;
using AdventureEngine.Models;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Services;

/// <summary>
/// Manages player conversation context for pronoun resolution and smart defaults
/// Phase 3: Context-aware parsing
/// </summary>
public class ContextManager
{
    private readonly AdventureDbContext _context;
    private readonly int _saveId;

    public ContextManager(AdventureDbContext context, int saveId)
    {
        _context = context;
        _saveId = saveId;
    }

    /// <summary>
    /// Get or create the player context for this save
    /// </summary>
    private async Task<PlayerContext> GetContextAsync()
    {
        var context = await _context.Set<PlayerContext>()
            .FirstOrDefaultAsync(pc => pc.GameSaveId == _saveId);

        if (context == null)
        {
            context = new PlayerContext
            {
                GameSaveId = _saveId,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Set<PlayerContext>().Add(context);
            await _context.SaveChangesAsync();
        }

        return context;
    }

    /// <summary>
    /// Update the last mentioned item (for "it", "that" references)
    /// </summary>
    public async Task SetLastMentionedItemAsync(int itemId)
    {
        var context = await GetContextAsync();
        context.LastMentionedItemId = itemId;
        context.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Update the last examined object
    /// </summary>
    public async Task SetLastExaminedObjectAsync(int examinableObjectId)
    {
        var context = await GetContextAsync();
        context.LastExaminedObjectId = examinableObjectId;
        context.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Update the last room (for "go back" functionality)
    /// </summary>
    public async Task SetLastRoomAsync(int roomId)
    {
        var context = await GetContextAsync();
        context.LastRoomId = roomId;
        context.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Get the last mentioned item ID (for pronoun resolution)
    /// Returns null if context expired (>5 minutes old)
    /// </summary>
    public async Task<int?> GetLastMentionedItemIdAsync()
    {
        var context = await GetContextAsync();

        // Context expires after 5 minutes
        if (context.UpdatedAt < DateTime.UtcNow.AddMinutes(-5))
        {
            return null;
        }

        return context.LastMentionedItemId;
    }

    /// <summary>
    /// Get the last mentioned item
    /// </summary>
    public async Task<Item?> GetLastMentionedItemAsync()
    {
        var itemId = await GetLastMentionedItemIdAsync();
        if (itemId == null)
        {
            return null;
        }

        return await _context.Items.FindAsync(itemId.Value);
    }

    /// <summary>
    /// Get the last room ID (for "go back")
    /// </summary>
    public async Task<int?> GetLastRoomIdAsync()
    {
        var context = await GetContextAsync();

        // Context expires after 10 minutes
        if (context.UpdatedAt < DateTime.UtcNow.AddMinutes(-10))
        {
            return null;
        }

        return context.LastRoomId;
    }

    /// <summary>
    /// Clear the context (useful when context becomes invalid)
    /// </summary>
    public async Task ClearContextAsync()
    {
        var context = await GetContextAsync();
        context.LastMentionedItemId = null;
        context.LastExaminedObjectId = null;
        context.LastRoomId = null;
        context.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
