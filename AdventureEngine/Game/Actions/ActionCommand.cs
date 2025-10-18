using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class ActionCommand : IGameCommand
{
    public string Name => "action";
    public string Description => "Perform a special action in this room";
    public string[] Aliases => ["do", "perform"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args)
    {
        if (args.Length == 0)
        {
            // List available actions
            var room = await gameState.GetCurrentRoomAsync();
            var actions = await gameState.Context.RoomActions
                .Where(ra => ra.RoomId == room!.Id)
                .ToListAsync();

            if (actions.Count == 0)
            {
                return CommandResult.Error("There are no special actions available here.");
            }

            var message = "Available actions:\n";
            foreach (var action in actions)
            {
                message += $"  - {action.ActionName}: {action.Description}\n";
            }
            return CommandResult.Ok(message.TrimEnd());
        }

        var actionName = string.Join(" ", args).ToLower();
        var currentRoom = await gameState.GetCurrentRoomAsync();

        var roomAction = await gameState.Context.RoomActions
            .Include(ra => ra.RequiredItem)
            .Include(ra => ra.UnlocksRoom)
            .FirstOrDefaultAsync(ra => ra.RoomId == currentRoom!.Id &&
                                      ra.ActionName.ToLower() == actionName);

        if (roomAction == null)
        {
            return CommandResult.Error($"You can't '{actionName}' here.");
        }

        // Check if action was already completed and is not repeatable
        if (!roomAction.IsRepeatable)
        {
            var alreadyCompleted = await gameState.Context.CompletedActions
                .AnyAsync(ca => ca.GameSaveId == gameState.CurrentSaveId &&
                               ca.RoomActionId == roomAction.Id);

            if (alreadyCompleted)
            {
                return CommandResult.Error("You've already done that.");
            }
        }

        // Check if required item is in inventory
        if (roomAction.RequiredItemId.HasValue)
        {
            var hasItem = await gameState.HasItemAsync(roomAction.RequiredItemId.Value);
            if (!hasItem)
            {
                return CommandResult.Error(roomAction.FailureMessage ?? "You don't have what you need to do that.");
            }
        }

        // Execute the action
        await gameState.CompleteActionAsync(roomAction.Id);

        // If action unlocks a room, update the connection
        if (roomAction.UnlocksRoomId.HasValue)
        {
            currentRoom!.UpRoomId = roomAction.UnlocksRoomId.Value;
            await gameState.Context.SaveChangesAsync();
        }

        return CommandResult.Ok(roomAction.SuccessMessage ?? "Done!");
    }
}
