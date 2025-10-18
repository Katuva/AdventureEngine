using AdventureEngine.Models;
using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class DropCommand : IGameCommand
{
    public string Name => "drop";
    public string Description => "Drop an item from your inventory";
    public string[] Aliases => ["place", "put"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args)
    {
        if (args.Length == 0)
        {
            return CommandResult.Error("Drop what? Specify an item name.");
        }

        var itemName = string.Join(" ", args).ToLower();
        var room = await gameState.GetCurrentRoomAsync();

        if (room == null)
        {
            return CommandResult.Error("You seem to be nowhere. This is a bug!");
        }

        // Find the item in the player's inventory
        var inventoryItem = await gameState.Context.InventoryItems
            .Include(ii => ii.Item)
            .FirstOrDefaultAsync(ii => ii.GameSaveId == gameState.CurrentSaveId &&
                                      ii.Item.Name.ToLower().Contains(itemName));

        if (inventoryItem == null)
        {
            return CommandResult.Error($"You don't have '{itemName}' in your inventory.");
        }

        // Remove from inventory
        gameState.Context.InventoryItems.Remove(inventoryItem);

        // Add to placed items in current room
        var placedItem = new PlacedItem
        {
            GameSaveId = gameState.CurrentSaveId,
            ItemId = inventoryItem.ItemId,
            RoomId = room.Id,
            PlacedAt = DateTime.UtcNow
        };

        gameState.Context.PlacedItems.Add(placedItem);
        await gameState.Context.SaveChangesAsync();

        return CommandResult.Ok($"You drop the {inventoryItem.Item.Name}.");
    }
}
