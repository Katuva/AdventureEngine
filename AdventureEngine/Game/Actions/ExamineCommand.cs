using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class ExamineCommand : IGameCommand
{
    public string Name => "examine";
    public string Description => "Examine an object closely";
    public string[] Aliases => ["inspect", "look at", "check"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        if (input.DirectObjects.Count == 0)
        {
            return CommandResult.Error("Examine what? Specify an object.");
        }

        var objectName = input.DirectObjects[0].ToLower();
        var room = await gameState.GetCurrentRoomAsync();

        if (room == null)
        {
            return CommandResult.Error("You seem to be nowhere. This is a bug!");
        }

        // Use SemanticResolver to find examinable objects
        var resolver = new SemanticResolver(gameState.Context);
        var examinableObject = await resolver.ResolveExaminableObjectAsync(objectName, room.Id);

        // Check if hidden and not revealed yet
        if (examinableObject is { IsHidden: true })
        {
            if (examinableObject.RevealedByActionId.HasValue)
            {
                var isRevealed = await gameState.Context.CompletedActions
                    .AnyAsync(ca => ca.GameSaveId == gameState.CurrentSaveId &&
                                    ca.RoomActionId == examinableObject.RevealedByActionId.Value);

                if (!isRevealed)
                {
                    examinableObject = null; // Not revealed yet
                }
            }
            else
            {
                examinableObject = null; // Hidden with no reveal condition
            }
        }

        if (examinableObject != null)
        {
            return CommandResult.Ok(examinableObject.Description);
        }

        // Also allow examining items in the room or inventory using semantic resolver
        var item = await resolver.ResolveItemAsync(
            objectName,
            gameState,
            includeInventory: true,
            includeRoom: true);

        return item != null ? CommandResult.Ok(item.Description) : CommandResult.Error($"You don't see anything special about '{objectName}'.");
    }
}
