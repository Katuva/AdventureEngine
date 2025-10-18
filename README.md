# Adventure Engine - Text Adventure Game

A modular, extensible text adventure game engine built with C# .NET 9.0, Entity Framework Core, and Spectre.Console.

## Features

- **Rich Console UI** - Beautiful console interface using Spectre.Console
- **YAML Configuration** - Easy game configuration via YAML files
- **SQLite Database** - Persistent game world with rooms, items, and actions
- **Entity Framework** - Full ORM support with migrations
- **Multiple Save Slots** - Support for multiple game saves
- **Extensible Command System** - Easy to add new player commands
- **Game State Management** - Proper game state pattern implementation
- **Modular Architecture** - Clean separation of concerns with proper OOP

## Project Structure

```
AdventureEngine/
├── Config/              # Game configuration (YAML)
│   ├── GameConfiguration.cs
│   └── ConfigurationLoader.cs
├── Data/                # Database context and seeding
│   ├── AdventureDbContext.cs
│   ├── DbContextFactory.cs
│   └── DatabaseSeeder.cs
├── Game/                # Game logic
│   ├── GameEngine.cs
│   └── Actions/         # Command pattern implementations
│       ├── IGameCommand.cs
│       ├── CommandRegistry.cs
│       ├── LookCommand.cs
│       ├── MoveCommand.cs
│       ├── TakeCommand.cs
│       ├── InventoryCommand.cs
│       ├── UseCommand.cs
│       ├── ActionCommand.cs
│       ├── HelpCommand.cs
│       └── QuitCommand.cs
├── Models/              # Database models
│   ├── Room.cs
│   ├── Item.cs
│   ├── RoomAction.cs
│   ├── GameSave.cs
│   ├── InventoryItem.cs
│   └── CompletedAction.cs
├── Services/            # Business logic services
│   ├── GameStateManager.cs
│   └── SaveGameService.cs
├── UI/                  # User interface
│   ├── ConsoleUI.cs
│   └── MainMenu.cs
└── Program.cs           # Application entry point
```

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- SQLite (included with Entity Framework)

### Building and Running

1. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

2. **Create the database migration:**
   ```bash
   dotnet ef migrations add InitialCreate
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

The application will:
- Create a `game-config.yaml` file if it doesn't exist
- Create and seed the `adventure.db` SQLite database
- Launch the main menu

## Game Configuration

The game is configured via `game-config.yaml`:

```yaml
GameName: Mystery Mansion Adventure
GameDescription: Explore a mysterious mansion and uncover its secrets
Author: Adventure Engine
Version: 1.0.0
MaxInventoryWeight: 100
MaxSaveSlots: 5
DatabasePath: adventure.db
UI:
  TitleColor: cyan
  DescriptionColor: white
  ErrorColor: red
  SuccessColor: green
  WarningColor: yellow
  PromptColor: blue
```

## Available Commands

- **look** (l, examine) - Look around the current room
- **north/south/east/west/up/down** (n/s/e/w/u/d) - Move in a direction
- **take** (get, grab, pick) - Pick up an item
- **inventory** (i, inv) - Show your inventory
- **use** (activate) - Use an item from inventory
- **action** (do, perform) - Perform special actions in rooms
- **help** (?, commands) - Show available commands
- **quit** (exit, q) - Exit the game

## Adding New Commands

To add a new command, create a class implementing `IGameCommand`:

```csharp
public class MyCustomCommand : IGameCommand
{
    public string Name => "mycommand";
    public string Description => "Does something cool";
    public string[] Aliases => ["mc", "cool"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args)
    {
        // Your command logic here
        return CommandResult.Ok("Command executed!");
    }
}
```

Then register it in `GameEngine.InitializeCommands()`:

```csharp
_commandRegistry.RegisterCommand(new MyCustomCommand());
```

## Database Schema

The game uses the following main entities:

- **Room** - Game locations with navigation links
- **Item** - Objects that can be found or carried
- **RoomAction** - Special actions available in specific rooms
- **GameSave** - Saved game state
- **InventoryItem** - Items in player's inventory
- **CompletedAction** - Tracks completed actions per save

## The Sample Game

The seeded game includes:
- A mysterious mansion to explore
- Multiple interconnected rooms
- Items to collect (lantern, golden key, ancient book)
- A deadly cellar (requires the lantern to survive)
- A secret room with treasure (requires the golden key)
- Win and lose conditions

## Extending the Game

### Adding New Rooms

Edit `DatabaseSeeder.cs` to add new rooms, items, and actions. The seeder runs automatically on first launch.

### Custom Game Logic

- **GameStateManager** - Modify for custom state tracking
- **SaveGameService** - Extend for additional save features
- **Commands** - Add new commands for new gameplay mechanics

## Technical Details

- **Framework:** .NET 9.0
- **ORM:** Entity Framework Core 9.0
- **Database:** SQLite
- **UI Library:** Spectre.Console 0.52.0
- **Configuration:** YamlDotNet 16.2.1
- **Architecture:** Command Pattern, State Pattern, Repository Pattern

## License

This is a sample project for educational purposes.

## Future Enhancements

Potential features to add:
- NPCs (Non-player characters)
- Combat system
- Puzzle mechanics
- Time-based events
- Achievements system
- Sound effects
- Custom scripting for room events
- Multiplayer support
