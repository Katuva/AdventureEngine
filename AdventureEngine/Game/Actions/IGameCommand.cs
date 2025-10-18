using AdventureEngine.Services;

namespace AdventureEngine.Game.Actions;

/// <summary>
/// Interface for the Command pattern - all player actions implement this
/// </summary>
public interface IGameCommand
{
    string Name { get; }
    string Description { get; }
    string[] Aliases { get; }

    /// <summary>
    /// Execute with parsed structured input (Phase 1: Enhanced parser)
    /// </summary>
    Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input);

    /// <summary>
    /// Legacy execute method for backward compatibility
    /// Default implementation converts args to ParsedInput
    /// </summary>
    Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args)
    {
        var input = ParsedInput.CreateSimple(Name, args);
        return ExecuteAsync(gameState, input);
    }
}

/// <summary>
/// Result of executing a command
/// </summary>
public class CommandResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool ShouldQuit { get; set; }
    public bool GameWon { get; set; }
    public bool GameLost { get; set; }

    public static CommandResult Ok(string message) => new() { Success = true, Message = message };
    public static CommandResult Error(string message) => new() { Success = false, Message = message };
    public static CommandResult Quit() => new() { Success = true, ShouldQuit = true };
    public static CommandResult Win(string message) => new() { Success = true, GameWon = true, Message = message };
    public static CommandResult Lose(string message) => new() { Success = true, GameLost = true, Message = message };
}
