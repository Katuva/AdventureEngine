using AdventureEngine.Models;
using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class TakeCommand : IGameCommand
{
    public string Name => "take";
    public string Description => "Pick up an item";
    public string[] Aliases => ["get", "grab", "pick"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args)
    {
        if (args.Length == 0)
        {
            return CommandResult.Error("Take what? Specify an item name.");
        }

        var itemName = string.Join(" ", args).ToLower();
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

        // First, try to find the item in the room's original items
        var item = await gameState.Context.Items
            .FirstOrDefaultAsync(i => i.RoomId == room.Id &&
                                      i.Name.ToLower().Contains(itemName) &&
                                      !itemsInInventory.Contains(i.Id));

        // If not found, check placed items
        PlacedItem? placedItem = null;
        if (item == null)
        {
            placedItem = await gameState.Context.PlacedItems
                .Include(pi => pi.Item)
                .FirstOrDefaultAsync(pi => pi.GameSaveId == gameState.CurrentSaveId &&
                                          pi.RoomId == room.Id &&
                                          pi.Item.Name.ToLower().Contains(itemName));

            if (placedItem != null)
            {
                item = placedItem.Item;
            }
        }

        if (item == null)
        {
            return CommandResult.Error($"There is no '{itemName}' here.");
        }

        if (!item.IsCollectable)
        {
            return CommandResult.Error($"You can't take the {item.Name}.");
        }

        // If this was a placed item, remove it from placed items
        if (placedItem != null)
        {
            gameState.Context.PlacedItems.Remove(placedItem);
        }

        await gameState.AddItemToInventoryAsync(item.Id);
        return CommandResult.Ok($"You take the {item.Name}.");
    }
}
