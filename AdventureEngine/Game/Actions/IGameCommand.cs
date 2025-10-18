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

    Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args);
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
