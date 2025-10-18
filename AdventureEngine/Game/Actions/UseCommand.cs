using AdventureEngine.Models;
using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class UseCommand : IGameCommand
{
    public string Name => "use";
    public string Description => "Use an item from your inventory (optionally 'on' something)";
    public string[] Aliases => ["activate"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        if (input.DirectObjects.Count == 0)
        {
            return CommandResult.Error("Use what? Specify an item name.");
        }

        // Get item name and target from parsed input
        var itemName = input.DirectObjects[0].ToLower();
        var targetName = input.IndirectObject?.ToLower();

        // Use SemanticResolver to find the item in inventory
        var resolver = new SemanticResolver(gameState.Context);
        var item = await resolver.ResolveItemAsync(
            itemName,
            gameState,
            includeInventory: true,
            includeRoom: false);

        if (item == null)
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
            // First, check for examinable object interactions using resolver
            var examinableObject = await resolver.ResolveExaminableObjectAsync(targetName, room.Id);

            // Check if this examinable requires our item
            if (examinableObject != null && examinableObject.RequiredItemId != item.Id)
            {
                examinableObject = null; // Wrong item for this object
            }

            // Load the unlocks room if needed
            if (examinableObject is { UnlocksRoomId: not null })
            {
                await gameState.Context.Entry(examinableObject)
                    .Reference(eo => eo.UnlocksRoom)
                    .LoadAsync();
            }

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

                return CommandResult.Ok(examinableObject.SuccessMessage ?? $"You use the {item.Name} on the {examinableObject.Name}.");
            }

            // Check for room actions that match
            var completedActionIds = await gameState.Context.CompletedActions
                .Where(ca => ca.GameSaveId == gameState.CurrentSaveId)
                .Select(ca => ca.RoomActionId)
                .ToListAsync();

            var roomAction = await gameState.Context.RoomActions
                .FirstOrDefaultAsync(ra => ra.RoomId == room.Id &&
                                          ra.RequiredItemId == item.Id &&
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

            return CommandResult.Error($"You can't use the {item.Name} on {targetName}.");
        }

        // Default use behavior
        var message = item.UseMessage ?? $"You use the {item.Name}.";
        return CommandResult.Ok(message);
    }
}
