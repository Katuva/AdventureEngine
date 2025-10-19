using AdventureEngine.Models;
using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class TakeCommand : IGameCommand
{
    public string Name => "take";
    public string Description => "Pick up an item";
    public string[] Aliases => ["get", "grab", "pick"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        if (input.DirectObjects.Count == 0)
        {
            return CommandResult.Error("Take what? Specify an item name.");
        }

        // Support multiple objects: "take lamp and sword"
        if (input.HasMultipleObjects)
        {
            var results = new List<string>();
            foreach (var itemName in input.DirectObjects)
            {
                var result = await TakeSingleItem(gameState, itemName.ToLower());
                results.Add(result);
            }
            return CommandResult.Ok(string.Join("\n", results));
        }

        // Single object
        var singleResult = await TakeSingleItem(gameState, input.DirectObjects[0].ToLower());
        return CommandResult.Ok(singleResult);
    }

    private static async Task<string> TakeSingleItem(GameStateManager gameState, string itemName)
    {
        var room = await gameState.GetCurrentRoomAsync();

        if (room == null)
        {
            return "You seem to be nowhere. This is a bug!";
        }

        // Use SemanticResolver to find the item with adjective support
        var resolver = new SemanticResolver(gameState.Context);
        var item = await resolver.ResolveItemAsync(
            itemName,
            gameState,
            includeInventory: false,  // Don't include inventory when taking
            includeRoom: true);

        if (item == null)
        {
            return $"There is no '{itemName}' here.";
        }

        if (!item.IsCollectable)
        {
            return $"You can't take the {item.Name}.";
        }

        // Check if this is a placed item and remove it
        var placedItem = await gameState.Context.PlacedItems
            .FirstOrDefaultAsync(pi => pi.GameSaveId == gameState.CurrentSaveId &&
                                      pi.ItemId == item.Id &&
                                      pi.RoomId == room.Id);

        if (placedItem != null)
        {
            gameState.Context.PlacedItems.Remove(placedItem);
        }

        // Track that this item has been picked up (so it won't appear in original room anymore)
        var alreadyPickedUp = await gameState.Context.PickedUpItems
            .AnyAsync(pui => pui.GameSaveId == gameState.CurrentSaveId && pui.ItemId == item.Id);

        if (!alreadyPickedUp)
        {
            var pickedUpItem = new PickedUpItem
            {
                GameSaveId = gameState.CurrentSaveId,
                ItemId = item.Id,
                FirstPickedUpAt = DateTime.UtcNow
            };
            gameState.Context.PickedUpItems.Add(pickedUpItem);
        }

        await gameState.AddItemToInventoryAsync(item.Id);

        // Check if picking up this item reveals any hidden objects
        var revealMessages = await gameState.CheckAndRevealExaminableObjectsAsync(
            triggeredByItemId: item.Id);

        var response = $"Taken: {item.Name}";

        // Append reveal messages if any
        if (revealMessages.Count > 0)
        {
            response += "\n\n" + string.Join("\n", revealMessages);
        }

        return response;
    }
}
