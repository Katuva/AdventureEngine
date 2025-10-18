using AdventureEngine.Config;
using AdventureEngine.Data;
using AdventureEngine.Models;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Services;

/// <summary>
/// Handles save/load operations for multiple game slots
/// </summary>
public class SaveGameService(AdventureDbContext context, GameConfiguration config)
{
    public async Task<List<GameSave>> GetAllSavesAsync()
    {
        return await context.GameSaves
            .Include(gs => gs.CurrentRoom)
            .OrderBy(gs => gs.SlotName)
            .ToListAsync();
    }

    public async Task<GameSave?> GetSaveAsync(int id)
    {
        return await context.GameSaves
            .Include(gs => gs.CurrentRoom)
            .Include(gs => gs.Inventory)
            .ThenInclude(ii => ii.Item)
            .FirstOrDefaultAsync(gs => gs.Id == id);
    }

    public async Task<GameSave?> GetSaveBySlotNameAsync(string slotName)
    {
        return await context.GameSaves
            .Include(gs => gs.CurrentRoom)
            .FirstOrDefaultAsync(gs => gs.SlotName == slotName);
    }

    public async Task<GameSave> CreateNewGameAsync(string slotName)
    {
        // Check if slot name already exists
        var existing = await GetSaveBySlotNameAsync(slotName);
        if (existing != null)
        {
            throw new InvalidOperationException($"Save slot '{slotName}' already exists");
        }

        // Get starting room
        var startingRoom = await context.Rooms
            .FirstOrDefaultAsync(r => r.IsStartingRoom);

        if (startingRoom == null)
        {
            throw new InvalidOperationException("No starting room found in the database");
        }

        var newSave = new GameSave
        {
            SlotName = slotName,
            CurrentRoomId = startingRoom.Id,
            SavedAt = DateTime.UtcNow,
            TurnCount = 0,
            Score = 0,
            Health = config.StartingHealth,
            IsCompleted = false,
            IsPlayerDead = false
        };

        context.GameSaves.Add(newSave);
        await context.SaveChangesAsync();

        return newSave;
    }

    public async Task UpdateSaveAsync(int saveId)
    {
        var save = await context.GameSaves.FindAsync(saveId);
        if (save != null)
        {
            save.SavedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteSaveAsync(int saveId)
    {
        var save = await context.GameSaves.FindAsync(saveId);
        if (save != null)
        {
            context.GameSaves.Remove(save);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasSavesAsync()
    {
        return await context.GameSaves.AnyAsync();
    }
}
