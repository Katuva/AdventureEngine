using AdventureEngine.Models;
using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class UseCommand : IGameCommand
{
    public string Name => "use";
    public string Description => "Use an item from your inventory (optionally 'on' something)";
    public string[] Aliases => ["activate"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args)
    {
        if (args.Length == 0)
        {
            return CommandResult.Error("Use what? Specify an item name.");
        }

        var fullInput = string.Join(" ", args).ToLower();

        // Check if using "use [item] on [target]" syntax
        string itemName;
        string? targetName = null;

        if (fullInput.Contains(" on "))
        {
            var parts = fullInput.Split(" on ", 2);
            itemName = parts[0].Trim();
            targetName = parts[1].Trim();
        }
        else
        {
            itemName = fullInput;
        }

        var inventoryItem = await gameState.Context.InventoryItems
            .Include(ii => ii.Item)
            .FirstOrDefaultAsync(ii => ii.GameSaveId == gameState.CurrentSaveId &&
                                      ii.Item.Name.ToLower().Contains(itemName));

        if (inventoryItem == null)
        {
            return CommandResult.Error($"You don't have '{itemName}' in your inventory.");
        }

        var room = await gameState.GetCurrentRoomAsync();
        if (room == null)
        {
            return CommandResult.Error("You seem to be nowhere. This is a bug!");
        }

        // If using on a target, check for interactions
        if (targetName != null)
        {
            // First, check for examinable object interactions
            var examinableObject = await gameState.Context.ExaminableObjects
                .Include(eo => eo.UnlocksRoom)
                .FirstOrDefaultAsync(eo => eo.RoomId == room.Id &&
                                          eo.RequiredItemId == inventoryItem.ItemId &&
                                          (eo.Name.ToLower().Contains(targetName) ||
                                           (eo.Keywords != null && eo.Keywords.ToLower().Contains(targetName))));

            if (examinableObject != null)
            {
                // Check if already completed for this save
                var alreadyCompleted = await gameState.Context.CompletedExaminableInteractions
                    .AnyAsync(cei => cei.GameSaveId == gameState.CurrentSaveId &&
                                    cei.ExaminableObjectId == examinableObject.Id);

                if (alreadyCompleted)
                {
                    return CommandResult.Error("You've already done that.");
                }

                // Mark as completed for this save
                var completedInteraction = new CompletedExaminableInteraction
                {
                    GameSaveId = gameState.CurrentSaveId,
                    ExaminableObjectId = examinableObject.Id,
                    CompletedAt = DateTime.UtcNow
                };
                gameState.Context.CompletedExaminableInteractions.Add(completedInteraction);
                await gameState.Context.SaveChangesAsync();

                return CommandResult.Ok(examinableObject.SuccessMessage ?? $"You use the {inventoryItem.Item.Name} on the {examinableObject.Name}.");
            }

            // Check for room actions that match
            var completedActionIds = await gameState.Context.CompletedActions
                .Where(ca => ca.GameSaveId == gameState.CurrentSaveId)
                .Select(ca => ca.RoomActionId)
                .ToListAsync();

            var roomAction = await gameState.Context.RoomActions
                .FirstOrDefaultAsync(ra => ra.RoomId == room.Id &&
                                          ra.RequiredItemId == inventoryItem.ItemId &&
                                          !completedActionIds.Contains(ra.Id) &&
                                          (ra.ActionName.ToLower().Contains(targetName) ||
                                           ra.Description.ToLower().Contains(targetName)));

            if (roomAction != null)
            {
                // Unlock room if applicable
                if (roomAction.UnlocksRoomId.HasValue)
                {
                    var unlockRoom = await gameState.Context.Rooms.FindAsync(roomAction.UnlocksRoomId.Value);
                    if (unlockRoom != null)
                    {
                        // Update the room connection
                        room.UpRoomId = roomAction.UnlocksRoomId.Value;
                        await gameState.Context.SaveChangesAsync();
                    }
                }

                // Mark action as completed
                await gameState.CompleteActionAsync(roomAction.Id);

                return CommandResult.Ok(roomAction.SuccessMessage ?? "Success!");
            }

            return CommandResult.Error($"You can't use the {inventoryItem.Item.Name} on {targetName}.");
        }

        // Default use behavior
        var message = inventoryItem.Item.UseMessage ?? $"You use the {inventoryItem.Item.Name}.";
        return CommandResult.Ok(message);
    }
}
