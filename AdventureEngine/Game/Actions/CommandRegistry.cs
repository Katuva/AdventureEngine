using AdventureEngine.Services;

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
        var lowerName = name.ToLower();

        // Try exact match first
        return _commands.TryGetValue(lowerName, out var command) ? command :
            // Try fuzzy matching if exact match fails
            GetCommandFuzzy(lowerName);
    }

    /// <summary>
    /// Find a command using fuzzy matching (typo tolerance)
    /// </summary>
    private IGameCommand? GetCommandFuzzy(string name)
    {
        var bestMatch = (Command: (IGameCommand?)null, Distance: int.MaxValue);

        foreach (var (key, cmd) in _commands)
        {
            var distance = FuzzyMatcher.LevenshteinDistance(name, key);

            // Only consider if distance is 2 or less and better than current best
            if (distance <= 2 && distance < bestMatch.Distance)
            {
                bestMatch = (cmd, distance);
            }
        }

        return bestMatch.Command;
    }

    public IEnumerable<IGameCommand> GetAllCommands()
    {
        return _commands.Values.Distinct();
    }
}
