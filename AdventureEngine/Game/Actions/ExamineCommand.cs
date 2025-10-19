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

        // Use SemanticResolver to find examinable objects (respects IsHidden)
        var resolver = new SemanticResolver(gameState.Context);
        var examinableObject = await resolver.ResolveExaminableObjectAsync(objectName, room.Id, gameState);

        if (examinableObject != null)
        {
            // Trigger any reveals that examining this object might cause
            var revealMessages = await gameState.CheckAndRevealExaminableObjectsAsync(
                triggeredByExaminableId: examinableObject.Id);

            var response = examinableObject.Description;

            // Append reveal messages if any
            if (revealMessages.Count > 0)
            {
                response += "\n\n" + string.Join("\n", revealMessages);
            }

            return CommandResult.Ok(response);
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
