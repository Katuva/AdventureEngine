using AdventureEngine.Data;
using AdventureEngine.Models;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Services;

/// <summary>
/// Manages the current game state including player position, inventory, and progress
/// </summary>
public class GameStateManager
{
    public AdventureDbContext Context { get; }
    public int CurrentSaveId { get; private set; }
    private Room? _currentRoom;

    public GameStateManager(AdventureDbContext context)
    {
        Context = context;
    }

    public async Task LoadGameAsync(int saveId)
    {
        CurrentSaveId = saveId;
        var save = await Context.GameSaves
            .Include(gs => gs.CurrentRoom)
            .FirstOrDefaultAsync(gs => gs.Id == saveId);

        if (save == null)
        {
            throw new InvalidOperationException($"Save game {saveId} not found");
        }

        _currentRoom = save.CurrentRoom;
    }

    public async Task<Room?> GetCurrentRoomAsync()
    {
        if (_currentRoom == null)
        {
            var save = await Context.GameSaves
                .Include(gs => gs.CurrentRoom)
                .FirstOrDefaultAsync(gs => gs.Id == CurrentSaveId);

            _currentRoom = save?.CurrentRoom;
        }

        return _currentRoom;
    }

    public async Task MoveToRoomAsync(int roomId)
    {
        var room = await Context.Rooms.FindAsync(roomId);
        if (room == null)
        {
            throw new InvalidOperationException($"Room {roomId} not found");
        }

        _currentRoom = room;

        var save = await Context.GameSaves.FindAsync(CurrentSaveId);
        if (save != null)
        {
            save.CurrentRoomId = roomId;
            save.TurnCount++;
            await Context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasItemAsync(int itemId)
    {
        return await Context.InventoryItems
            .AnyAsync(ii => ii.GameSaveId == CurrentSaveId && ii.ItemId == itemId);
    }

    public async Task AddItemToInventoryAsync(int itemId)
    {
        var item = await Context.Items.FindAsync(itemId);
        if (item == null)
        {
            throw new InvalidOperationException($"Item {itemId} not found");
        }

        // Check if already in inventory
        var alreadyHave = await Context.InventoryItems
            .AnyAsync(ii => ii.GameSaveId == CurrentSaveId && ii.ItemId == itemId);

        if (alreadyHave)
        {
            throw new InvalidOperationException("Item already in inventory");
        }

        // Add to inventory (don't modify item.RoomId - keep it for other saves)
        var inventoryItem = new InventoryItem
        {
            GameSaveId = CurrentSaveId,
            ItemId = itemId,
            PickedUpAt = DateTime.UtcNow
        };

        Context.InventoryItems.Add(inventoryItem);
        await Context.SaveChangesAsync();
    }

    public async Task CompleteActionAsync(int roomActionId)
    {
        var completedAction = new CompletedAction
        {
            GameSaveId = CurrentSaveId,
            RoomActionId = roomActionId,
            CompletedAt = DateTime.UtcNow
        };

        Context.CompletedActions.Add(completedAction);

        // Award points
        var save = await Context.GameSaves.FindAsync(CurrentSaveId);
        if (save != null)
        {
            save.Score += 10;
        }

        await Context.SaveChangesAsync();
    }

    public async Task<bool> CanSurviveDeadlyRoomAsync(int roomId)
    {
        // Check if there's a room action that allows surviving this room
        var roomActions = await Context.RoomActions
            .Where(ra => ra.RoomId == roomId && ra.RequiredItemId != null)
            .ToListAsync();

        foreach (var action in roomActions)
        {
            if (action.RequiredItemId.HasValue && await HasItemAsync(action.RequiredItemId.Value))
            {
                return true;
            }
        }

        return false;
    }

    public async Task MarkGameCompletedAsync(bool won)
    {
        var save = await Context.GameSaves.FindAsync(CurrentSaveId);
        if (save != null)
        {
            save.IsCompleted = true;
            save.IsPlayerDead = !won;
            if (won)
            {
                save.Score += 100; // Bonus for winning
            }
            await Context.SaveChangesAsync();
        }
    }

    public async Task<int> GetHealthAsync()
    {
        var save = await Context.GameSaves.FindAsync(CurrentSaveId);
        return save?.Health ?? 0;
    }

    public async Task ModifyHealthAsync(int amount)
    {
        var save = await Context.GameSaves.FindAsync(CurrentSaveId);
        if (save != null)
        {
            save.Health += amount;
            if (save.Health < 0)
            {
                save.Health = 0;
            }
            await Context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsPlayerDeadAsync()
    {
        var health = await GetHealthAsync();
        return health <= 0;
    }
}
