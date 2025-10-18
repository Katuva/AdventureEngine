using AdventureEngine.Data;
using AdventureEngine.Models;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Services;

/// <summary>
/// Manages player conversation context for pronoun resolution and smart defaults
/// Phase 3: Context-aware parsing
/// </summary>
public class ContextManager(AdventureDbContext context, int saveId)
{
    /// <summary>
    /// Get or create the player context for this save
    /// </summary>
    private async Task<PlayerContext> GetContextAsync()
    {
        var context1 = await context.Set<PlayerContext>()
            .FirstOrDefaultAsync(pc => pc.GameSaveId == saveId);

        if (context1 != null) return context1;
        
        context1 = new PlayerContext
        {
            GameSaveId = saveId,
            UpdatedAt = DateTime.UtcNow
        };
        context.Set<PlayerContext>().Add(context1);
        await context.SaveChangesAsync();

        return context1;
    }

    /// <summary>
    /// Update the last mentioned item (for "it", "that" references)
    /// </summary>
    public async Task SetLastMentionedItemAsync(int itemId)
    {
        var context1 = await GetContextAsync();
        context1.LastMentionedItemId = itemId;
        context1.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Update the last examined object
    /// </summary>
    public async Task SetLastExaminedObjectAsync(int examinableObjectId)
    {
        var context1 = await GetContextAsync();
        context1.LastExaminedObjectId = examinableObjectId;
        context1.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Update the last room (for "go back" functionality)
    /// </summary>
    public async Task SetLastRoomAsync(int roomId)
    {
        var context1 = await GetContextAsync();
        context1.LastRoomId = roomId;
        context1.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Get the last mentioned item ID (for pronoun resolution)
    /// Returns null if context expired (>5 minutes old)
    /// </summary>
    public async Task<int?> GetLastMentionedItemIdAsync()
    {
        var context = await GetContextAsync();

        // Context expires after 5 minutes
        return context.UpdatedAt < DateTime.UtcNow.AddMinutes(-5) ? null : context.LastMentionedItemId;
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

        return await context.Items.FindAsync(itemId.Value);
    }

    /// <summary>
    /// Get the last room ID (for "go back")
    /// </summary>
    public async Task<int?> GetLastRoomIdAsync()
    {
        var playerContext = await GetContextAsync();

        // Context expires after 10 minutes
        return playerContext.UpdatedAt < DateTime.UtcNow.AddMinutes(-10) ? null : playerContext.LastRoomId;
    }

    /// <summary>
    /// Clear the context (useful when context becomes invalid)
    /// </summary>
    public async Task ClearContextAsync()
    {
        var context1 = await GetContextAsync();
        context1.LastMentionedItemId = null;
        context1.LastExaminedObjectId = null;
        context1.LastRoomId = null;
        context1.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }
}
