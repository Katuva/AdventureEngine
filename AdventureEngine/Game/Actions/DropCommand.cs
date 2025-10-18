using AdventureEngine.Models;
using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class DropCommand : IGameCommand
{
    public string Name => "drop";
    public string Description => "Drop an item from your inventory";
    public string[] Aliases => ["place", "put"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        if (input.DirectObjects.Count == 0)
        {
            return CommandResult.Error("Drop what? Specify an item name.");
        }

        var itemName = input.DirectObjects[0].ToLower();

        // Note: Preposition (in/on) is parsed but not used yet in Phase 1
        // Phase 2 will add container support to actually put items IN/ON things
        // For now "drop lamp" and "put lamp on table" both just drop it in the room
        var room = await gameState.GetCurrentRoomAsync();

        if (room == null)
        {
            return CommandResult.Error("You seem to be nowhere. This is a bug!");
        }

        // Use SemanticResolver to find the item in inventory
        var resolver = new SemanticResolver(gameState.Context);
        var item = await resolver.ResolveItemAsync(
            itemName,
            gameState,
            includeInventory: true,
            includeRoom: false);  // Only check inventory when dropping

        if (item == null)
        {
            return CommandResult.Error($"You don't have '{itemName}' in your inventory.");
        }

        // Find the inventory item record
        var inventoryItem = await gameState.Context.InventoryItems
            .FirstOrDefaultAsync(ii => ii.GameSaveId == gameState.CurrentSaveId &&
                                      ii.ItemId == item.Id);

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

        return CommandResult.Ok($"You drop the {item.Name}.");
    }
}
