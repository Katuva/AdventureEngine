using AdventureEngine.Config;
using AdventureEngine.Data;
using AdventureEngine.Models;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Services;

/// <summary>
/// Handles save/load operations for multiple game slots
/// </summary>
public class SaveGameService
{
    private readonly AdventureDbContext _context;
    private readonly GameConfiguration _config;

    public SaveGameService(AdventureDbContext context, GameConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<List<GameSave>> GetAllSavesAsync()
    {
        return await _context.GameSaves
            .Include(gs => gs.CurrentRoom)
            .OrderBy(gs => gs.SlotName)
            .ToListAsync();
    }

    public async Task<GameSave?> GetSaveAsync(int id)
    {
        return await _context.GameSaves
            .Include(gs => gs.CurrentRoom)
            .Include(gs => gs.Inventory)
            .ThenInclude(ii => ii.Item)
            .FirstOrDefaultAsync(gs => gs.Id == id);
    }

    public async Task<GameSave?> GetSaveBySlotNameAsync(string slotName)
    {
        return await _context.GameSaves
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
        var startingRoom = await _context.Rooms
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
            Health = _config.StartingHealth,
            IsCompleted = false,
            IsPlayerDead = false
        };

        _context.GameSaves.Add(newSave);
        await _context.SaveChangesAsync();

        return newSave;
    }

    public async Task UpdateSaveAsync(int saveId)
    {
        var save = await _context.GameSaves.FindAsync(saveId);
        if (save != null)
        {
            save.SavedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteSaveAsync(int saveId)
    {
        var save = await _context.GameSaves.FindAsync(saveId);
        if (save != null)
        {
            _context.GameSaves.Remove(save);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasSavesAsync()
    {
        return await _context.GameSaves.AnyAsync();
    }
}
