using AdventureEngine.Config;
using AdventureEngine.Data;
using AdventureEngine.Game.Actions;
using AdventureEngine.Services;
using AdventureEngine.UI;
using Spectre.Console;

namespace AdventureEngine.Game;

/// <summary>
/// Main game engine that orchestrates gameplay
/// </summary>
public class GameEngine
{
    private readonly AdventureDbContext _context;
    private readonly ConsoleUI _ui;
    private readonly GameConfiguration _config;
    private readonly CommandRegistry _commandRegistry;
    private readonly CommandParser _commandParser;
    private GameStateManager _gameState = null!;

    public GameEngine(AdventureDbContext context, ConsoleUI ui, GameConfiguration config)
    {
        _context = context;
        _ui = ui;
        _config = config;
        _commandRegistry = new CommandRegistry();
        _commandParser = new CommandParser();
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        // Register all available commands
        _commandRegistry.RegisterCommand(new LookCommand());
        _commandRegistry.RegisterCommand(new ExamineCommand());
        _commandRegistry.RegisterCommand(new MoveCommand("north"));
        _commandRegistry.RegisterCommand(new MoveCommand("south"));
        _commandRegistry.RegisterCommand(new MoveCommand("east"));
        _commandRegistry.RegisterCommand(new MoveCommand("west"));
        _commandRegistry.RegisterCommand(new MoveCommand("up"));
        _commandRegistry.RegisterCommand(new MoveCommand("down"));
        _commandRegistry.RegisterCommand(new TakeCommand());
        _commandRegistry.RegisterCommand(new DropCommand());
        _commandRegistry.RegisterCommand(new InventoryCommand());
        _commandRegistry.RegisterCommand(new UseCommand());
        _commandRegistry.RegisterCommand(new ActionCommand());
        _commandRegistry.RegisterCommand(new LightCommand());
        _commandRegistry.RegisterCommand(new ExtinguishCommand());
        _commandRegistry.RegisterCommand(new HelpCommand(_commandRegistry));
        _commandRegistry.RegisterCommand(new QuitCommand());

        // You can easily add more commands here!
        // _commandRegistry.RegisterCommand(new YourCustomCommand());
    }

    public async Task RunAsync(int saveId)
    {
        _gameState = new GameStateManager(_context);
        await _gameState.LoadGameAsync(saveId);

        AnsiConsole.Clear();
        _ui.ShowTitle();

        // Show initial room
        var initialRoom = await _gameState.GetCurrentRoomAsync();
        if (initialRoom != null)
        {
            _ui.ShowRoomHeader(initialRoom.Name);
            _ui.ShowMessage(initialRoom.Description);
            AnsiConsole.WriteLine();
        }

        // Game loop
        var running = true;
        while (running)
        {
            try
            {
                // Show compass and health before input prompt
                var currentRoom = await _gameState.GetCurrentRoomAsync();

                if (currentRoom != null)
                {
                    _ui.ShowCompass(currentRoom);
                    AnsiConsole.WriteLine();
                }

                var currentHealth = await _gameState.GetHealthAsync();
                _ui.ShowHealthBar(currentHealth, _config.StartingHealth);
                AnsiConsole.WriteLine();

                var input = _ui.GetInput();

                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                // Parse the input using enhanced parser
                var parsedInput = _commandParser.Parse(input);

                var command = _commandRegistry.GetCommand(parsedInput.Verb);

                if (command == null)
                {
                    _ui.ShowError($"Unknown command: {parsedInput.Verb}. Type 'help' for available commands.");
                    continue;
                }

                var result = await command.ExecuteAsync(_gameState, parsedInput);

                // Clear screen after command execution
                AnsiConsole.Clear();

                // Show current room info after clearing
                var currentRoomAfterCommand = await _gameState.GetCurrentRoomAsync();
                if (currentRoomAfterCommand != null && result.Success)
                {
                    _ui.ShowRoomHeader(currentRoomAfterCommand.Name);
                }

                // Display command result message
                if (!string.IsNullOrWhiteSpace(result.Message))
                {
                    _ui.ShowMessage(result.Message);
                    AnsiConsole.WriteLine();
                }

                // Handle game state changes
                if (result.ShouldQuit)
                {
                    running = false;
                }
                else if (result.GameWon)
                {
                    await _gameState.MarkGameCompletedAsync(true);
                    _ui.ShowGameOver(true, result.Message);
                    running = false;
                }
                else if (result.GameLost)
                {
                    await _gameState.MarkGameCompletedAsync(false);
                    _ui.ShowGameOver(false, result.Message);
                    running = false;
                }
            }
            catch (Exception ex)
            {
                _ui.ShowError($"An error occurred: {ex.Message}");
                AnsiConsole.WriteException(ex);
            }
        }

        // Auto-save when exiting
        var saveService = new SaveGameService(_context, _config);
        await saveService.UpdateSaveAsync(saveId);
    }
}
