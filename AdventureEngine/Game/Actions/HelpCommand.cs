using AdventureEngine.Services;

namespace AdventureEngine.Game.Actions;

public class HelpCommand(CommandRegistry registry) : IGameCommand
{
    public string Name => "help";
    public string Description => "Show available commands";
    public string[] Aliases => ["?", "commands"];

    public Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        var message = "Available commands:\n\n";

        foreach (var command in registry.GetAllCommands())
        {
            message += $"{command.Name,-15} - {command.Description}";
            if (command.Aliases.Length > 0)
            {
                message += $" (aliases: {string.Join(", ", command.Aliases)})";
            }
            message += "\n";
        }

        return Task.FromResult(CommandResult.Ok(message));
    }
}
