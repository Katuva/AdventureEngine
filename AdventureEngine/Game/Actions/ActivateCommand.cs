using AdventureEngine.Services;

namespace AdventureEngine.Game.Actions;

public class ActivateCommand : IGameCommand
{
    public string Name => "activate";
    public string Description => "Activate, press, or toggle a switch, lever, or button";
    public string[] Aliases => ["press", "toggle", "flip", "pull", "push", "trigger"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        if (input.DirectObjects.Count == 0)
        {
            return CommandResult.Error("Activate what? Specify an object (e.g., 'activate switch').");
        }

        var objectName = input.DirectObjects[0].ToLower();
        var room = await gameState.GetCurrentRoomAsync();

        if (room == null)
        {
            return CommandResult.Error("You seem to be nowhere. This is a bug!");
        }

        // Use SemanticResolver to find the examinable object
        var resolver = new SemanticResolver(gameState.Context);
        var examinableObject = await resolver.ResolveExaminableObjectAsync(objectName, room.Id, gameState);

        if (examinableObject == null)
        {
            return CommandResult.Error($"You don't see any '{objectName}' here that you can activate.");
        }

        // Check if object is activatable
        if (!examinableObject.IsActivatable)
        {
            return CommandResult.Error($"You can't activate the {examinableObject.Name}.");
        }

        // Check if object has limited uses
        if (examinableObject.MaxUses > 0)
        {
            var usageCount = await gameState.GetExaminableObjectUsageCountAsync(examinableObject.Id);
            if (usageCount >= examinableObject.MaxUses)
            {
                // Show empty description if available, otherwise generic message
                var message = !string.IsNullOrEmpty(examinableObject.EmptyDescription)
                    ? examinableObject.EmptyDescription
                    : $"The {examinableObject.Name} has already been used.";
                return CommandResult.Error(message);
            }
        }

        // Activate the object
        try
        {
            var (activationMessage, revealMessages) = await gameState.ActivateExaminableObjectAsync(examinableObject.Id);

            var response = activationMessage;

            // Append reveal messages if any
            if (revealMessages.Count > 0)
            {
                response += "\n\n" + string.Join("\n", revealMessages);
            }

            return CommandResult.Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return CommandResult.Error(ex.Message);
        }
    }
}
