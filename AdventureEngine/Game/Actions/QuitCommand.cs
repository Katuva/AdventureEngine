using AdventureEngine.Services;

namespace AdventureEngine.Game.Actions;

public class QuitCommand : IGameCommand
{
    public string Name => "quit";
    public string Description => "Exit the game";
    public string[] Aliases => ["exit", "q"];

    public Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args)
    {
        return Task.FromResult(CommandResult.Quit());
    }
}
