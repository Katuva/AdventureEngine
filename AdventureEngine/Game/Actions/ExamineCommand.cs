using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class ExamineCommand : IGameCommand
{
    public string Name => "examine";
    public string Description => "Examine an object closely";
    public string[] Aliases => ["inspect", "look at", "check", "x"];

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

            // Also check if any containers should be revealed
            var containerRevealMessages = await gameState.CheckAndRevealContainersAsync(
                triggeredByExaminableId: examinableObject.Id);

            // Get description based on usage state
            var response = await gameState.GetExaminableObjectDescriptionAsync(examinableObject);

            // Append reveal messages if any
            if (revealMessages.Count > 0)
            {
                response += "\n\n" + string.Join("\n", revealMessages);
            }

            // Append container reveal messages if any
            if (containerRevealMessages.Count > 0)
            {
                response += "\n\n" + string.Join("\n", containerRevealMessages);
            }

            return CommandResult.Ok(response);
        }

        // Also allow examining items in the room or inventory using semantic resolver
        var item = await resolver.ResolveItemAsync(
            objectName,
            gameState,
            includeInventory: true,
            includeRoom: true);

        if (item != null)
        {
            // Get description based on usage state
            var description = await gameState.GetItemDescriptionAsync(item);
            return CommandResult.Ok(description);
        }

        // Check for containers
        var containers = await gameState.Context.Containers
            .Where(c => c.RoomId == room.Id)
            .ToListAsync();

        var container = containers.FirstOrDefault(c =>
            c.Name.ToLower() == objectName ||
            (c.Keywords != null && c.Keywords.ToLower().Split(',').Any(k => k.Trim() == objectName)));

        if (container != null)
        {
            var state = await gameState.Context.ContainerStates
                .FirstOrDefaultAsync(cs => cs.GameSaveId == gameState.CurrentSaveId && cs.ContainerId == container.Id);

            var response = container.Description;

            if (state != null && state.IsOpen)
            {
                var containerItems = await gameState.Context.ContainerItems
                    .Include(ci => ci.Item)
                    .Where(ci => ci.ContainerId == container.Id)
                    .ToListAsync();

                if (containerItems.Count > 0)
                {
                    var itemNames = string.Join(", ", containerItems.Select(ci => ci.Item.Name));
                    response += $"\n\nThe {container.Name} is open and contains: {itemNames}";
                }
                else
                {
                    var emptyMsg = container.EmptyDescription ?? "empty";
                    response += $"\n\nThe {container.Name} is open and {emptyMsg}";
                }
            }
            else if (state != null && state.IsLocked)
            {
                response += $"\n\nThe {container.Name} is locked.";
            }
            else
            {
                response += $"\n\nThe {container.Name} is closed.";
            }

            return CommandResult.Ok(response);
        }

        return CommandResult.Error($"You don't see anything special about '{objectName}'.");
    }
}
