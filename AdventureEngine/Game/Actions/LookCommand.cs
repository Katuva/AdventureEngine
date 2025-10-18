using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class LookCommand : IGameCommand
{
    public string Name => "look";
    public string Description => "Look around the current room";
    public string[] Aliases => ["l", "examine"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args)
    {
        var room = await gameState.GetCurrentRoomAsync();
        if (room == null)
        {
            return CommandResult.Error("You seem to be nowhere. This is a bug!");
        }

        // Get items in this save's inventory
        var itemsInInventory = await gameState.Context.InventoryItems
            .Where(ii => ii.GameSaveId == gameState.CurrentSaveId)
            .Select(ii => ii.ItemId)
            .ToListAsync();

        // Get original room items that aren't in inventory
        var originalItems = await gameState.Context.Items
            .Where(i => i.RoomId == room.Id && !itemsInInventory.Contains(i.Id))
            .ToListAsync();

        // Get items placed in this room by this save
        var placedItems = await gameState.Context.PlacedItems
            .Include(pi => pi.Item)
            .Where(pi => pi.GameSaveId == gameState.CurrentSaveId && pi.RoomId == room.Id)
            .Select(pi => pi.Item)
            .ToListAsync();

        var description = $"{room.Description}";

        // Combine original and placed items
        var allItems = originalItems.Concat(placedItems).ToList();

        if (allItems.Any())
        {
            description += "\n\nYou can see:";
            foreach (var item in allItems)
            {
                description += $"\n  - {item.Name}: {item.Description}";
            }
        }

        // Exits are now shown via the compass display
        return CommandResult.Ok(description);
    }
}
