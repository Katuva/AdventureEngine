namespace AdventureEngine.Game.Actions;

/// <summary>
/// Registry for all available game commands - makes it easy to add new commands
/// </summary>
public class CommandRegistry
{
    private readonly Dictionary<string, IGameCommand> _commands = new();

    public void RegisterCommand(IGameCommand command)
    {
        _commands[command.Name.ToLower()] = command;

        // Register aliases
        foreach (var alias in command.Aliases)
        {
            _commands[alias.ToLower()] = command;
        }
    }

    public IGameCommand? GetCommand(string name)
    {
        _commands.TryGetValue(name.ToLower(), out var command);
        return command;
    }

    public IEnumerable<IGameCommand> GetAllCommands()
    {
        return _commands.Values.Distinct();
    }
}
