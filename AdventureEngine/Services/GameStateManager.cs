using AdventureEngine.Data;
using AdventureEngine.Models;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Services;

/// <summary>
/// Manages the current game state including player position, inventory, and progress
/// </summary>
public class GameStateManager(AdventureDbContext context)
{
    public AdventureDbContext Context { get; } = context;
    public int CurrentSaveId { get; private set; }
    private Room? _currentRoom;

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

        // Mark starting room as visited if this is a new game
        await MarkRoomVisitedAsync(save.CurrentRoom.Id);
    }

    public async Task<Room?> GetCurrentRoomAsync()
    {
        if (_currentRoom != null) return _currentRoom;
        
        var save = await Context.GameSaves
            .Include(gs => gs.CurrentRoom)
            .FirstOrDefaultAsync(gs => gs.Id == CurrentSaveId);

        _currentRoom = save?.CurrentRoom;

        return _currentRoom;
    }

    public async Task MoveToRoomAsync(int roomId)
    {
        var room = await Context.Rooms.FindAsync(roomId);

        _currentRoom = room ?? throw new InvalidOperationException($"Room {roomId} not found");

        var save = await Context.GameSaves.FindAsync(CurrentSaveId);
        if (save != null)
        {
            save.CurrentRoomId = roomId;
            save.TurnCount++;
            await Context.SaveChangesAsync();
        }

        // Track room visit
        await MarkRoomVisitedAsync(roomId);
    }

    /// <summary>
    /// Mark a room as visited for this save
    /// </summary>
    public async Task MarkRoomVisitedAsync(int roomId)
    {
        var visited = await Context.VisitedRooms
            .FirstOrDefaultAsync(vr => vr.GameSaveId == CurrentSaveId && vr.RoomId == roomId);

        if (visited == null)
        {
            // First visit
            visited = new VisitedRoom
            {
                GameSaveId = CurrentSaveId,
                RoomId = roomId,
                FirstVisitedAt = DateTime.UtcNow,
                LastVisitedAt = DateTime.UtcNow,
                VisitCount = 1
            };
            Context.VisitedRooms.Add(visited);
        }
        else
        {
            // Subsequent visit
            visited.LastVisitedAt = DateTime.UtcNow;
            visited.VisitCount++;
        }

        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Check if a room has been visited by this save
    /// </summary>
    public async Task<bool> HasVisitedRoomAsync(int roomId)
    {
        return await Context.VisitedRooms
            .AnyAsync(vr => vr.GameSaveId == CurrentSaveId && vr.RoomId == roomId);
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
        var room = await Context.Rooms.FindAsync(roomId);
        if (room == null)
        {
            return false;
        }

        // Check if room has a specific protection item
        if (!room.ProtectionItemId.HasValue)
        {
            // No protection item defined - room is always deadly
            return false;
        }

        // Check if player has the protection item
        var hasItem = await HasItemAsync(room.ProtectionItemId.Value);
        if (!hasItem)
        {
            return false;
        }

        // If a specific state is required, check it
        if (!string.IsNullOrEmpty(room.RequiredItemState))
        {
            var isInRequiredState = await IsItemInStateAsync(room.ProtectionItemId.Value, room.RequiredItemState);
            return isInRequiredState;
        }

        // No specific state required - just having the item is enough
        return true;
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

    /// <summary>
    /// Get the current state of an item for this save
    /// </summary>
    public async Task<string> GetItemStateAsync(int itemId)
    {
        var itemState = await Context.ItemStates
            .FirstOrDefaultAsync(ist => ist.GameSaveId == CurrentSaveId && ist.ItemId == itemId);

        return itemState?.State ?? ItemStates.Default;
    }

    /// <summary>
    /// Set the state of an item for this save
    /// </summary>
    public async Task SetItemStateAsync(int itemId, string state)
    {
        var itemState = await Context.ItemStates
            .FirstOrDefaultAsync(ist => ist.GameSaveId == CurrentSaveId && ist.ItemId == itemId);

        if (itemState == null)
        {
            itemState = new ItemState
            {
                GameSaveId = CurrentSaveId,
                ItemId = itemId,
                State = state,
                UpdatedAt = DateTime.UtcNow
            };
            Context.ItemStates.Add(itemState);
        }
        else
        {
            itemState.State = state;
            itemState.UpdatedAt = DateTime.UtcNow;
        }

        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Check if an item is in a specific state
    /// </summary>
    public async Task<bool> IsItemInStateAsync(int itemId, string state)
    {
        var currentState = await GetItemStateAsync(itemId);
        return currentState.Equals(state, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Get all items visible in the current room (excludes inventory items)
    /// </summary>
    public async Task<List<Item>> GetRoomItemsAsync(int roomId)
    {
        // Get items in this save's inventory
        var itemsInInventory = await Context.InventoryItems
            .Where(ii => ii.GameSaveId == CurrentSaveId)
            .Select(ii => ii.ItemId)
            .ToListAsync();

        // Get original room items that aren't in inventory
        var originalItems = await Context.Items
            .Where(i => i.RoomId == roomId && !itemsInInventory.Contains(i.Id))
            .ToListAsync();

        // Get items placed in this room by this save
        var placedItems = await Context.PlacedItems
            .Include(pi => pi.Item)
            .Where(pi => pi.GameSaveId == CurrentSaveId && pi.RoomId == roomId)
            .Select(pi => pi.Item)
            .ToListAsync();

        return originalItems.Concat(placedItems).Distinct().ToList();
    }
}
