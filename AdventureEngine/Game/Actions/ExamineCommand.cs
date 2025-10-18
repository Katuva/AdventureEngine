using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class ExamineCommand : IGameCommand
{
    public string Name => "examine";
    public string Description => "Examine an object closely";
    public string[] Aliases => ["inspect", "look at", "check"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args)
    {
        if (args.Length == 0)
        {
            return CommandResult.Error("Examine what? Specify an object.");
        }

        var objectName = string.Join(" ", args).ToLower();
        var room = await gameState.GetCurrentRoomAsync();

        if (room == null)
        {
            return CommandResult.Error("You seem to be nowhere. This is a bug!");
        }

        // Get completed actions for this save to check what's been revealed
        var completedActionIds = await gameState.Context.CompletedActions
            .Where(ca => ca.GameSaveId == gameState.CurrentSaveId)
            .Select(ca => ca.RoomActionId)
            .ToListAsync();

        // Find examinable objects in the room
        var examinableObject = await gameState.Context.ExaminableObjects
            .Where(eo => eo.RoomId == room.Id)
            .Where(eo => !eo.IsHidden ||
                        (eo.RevealedByActionId.HasValue && completedActionIds.Contains(eo.RevealedByActionId.Value)))
            .FirstOrDefaultAsync(eo => eo.Name.ToLower().Contains(objectName) ||
                                      (eo.Keywords != null && eo.Keywords.ToLower().Contains(objectName)));

        if (examinableObject != null)
        {
            return CommandResult.Ok(examinableObject.Description);
        }

        // Also allow examining items in the room or inventory
        var itemsInInventory = await gameState.Context.InventoryItems
            .Where(ii => ii.GameSaveId == gameState.CurrentSaveId)
            .Select(ii => ii.ItemId)
            .ToListAsync();

        // Check room items
        var roomItem = await gameState.Context.Items
            .FirstOrDefaultAsync(i => (i.RoomId == room.Id || itemsInInventory.Contains(i.Id)) &&
                                     i.Name.ToLower().Contains(objectName));

        if (roomItem != null)
        {
            return CommandResult.Ok(roomItem.Description);
        }

        // Check placed items
        var placedItem = await gameState.Context.PlacedItems
            .Include(pi => pi.Item)
            .Where(pi => pi.GameSaveId == gameState.CurrentSaveId && pi.RoomId == room.Id)
            .FirstOrDefaultAsync(pi => pi.Item.Name.ToLower().Contains(objectName));

        if (placedItem != null)
        {
            return CommandResult.Ok(placedItem.Item.Description);
        }

        return CommandResult.Error($"You don't see anything special about '{objectName}'.");
    }
}
