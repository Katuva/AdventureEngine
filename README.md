# Adventure Engine - Text Adventure Game

A modular, extensible text adventure game engine built with C# .NET 9.0, Entity Framework Core, and Spectre.Console.

## Features

- **Rich Console UI** - Beautiful console interface using Spectre.Console with dynamic compass and health bar
- **YAML Configuration** - Easy game configuration via YAML files
- **SQLite Database** - Persistent game world with rooms, items, and actions
- **Entity Framework** - Full ORM support with migrations
- **Multiple Save Slots** - Support for multiple game saves with independent state tracking
- **Extensible Command System** - Easy to add new player commands
- **Game State Management** - Proper game state pattern implementation with per-save tracking
- **Modular Architecture** - Clean separation of concerns with proper OOP
- **Health System** - Track player health with deadly rooms and protective items
- **Item States** - Items can have states (lit/extinguished) that affect gameplay
- **Examinable Objects** - Rich object examination system with keywords and context
- **Hidden Objects & Reveals** - Objects can be hidden and revealed by examining other objects, picking up items, or completing actions
- **Activation System** - Switches, levers, and buttons that can trigger events across different rooms
- **Dynamic Room Descriptions** - Room descriptions that change based on game state and player inventory
- **Semantic Matching** - Fuzzy string matching and adjective support for natural language commands
- **Progressive Exploration** - Compass shows visited vs. unvisited rooms, locked vs. unlocked paths
- **Quest Items** - Special items required to unlock areas and progress the story

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
│       ├── CommandParser.cs
│       ├── LookCommand.cs
│       ├── ExamineCommand.cs
│       ├── MoveCommand.cs
│       ├── TakeCommand.cs
│       ├── DropCommand.cs
│       ├── InventoryCommand.cs
│       ├── UseCommand.cs
│       ├── ActionCommand.cs
│       ├── LightCommand.cs
│       ├── ExtinguishCommand.cs
│       ├── ActivateCommand.cs
│       ├── HelpCommand.cs
│       └── QuitCommand.cs
├── Models/              # Database models
│   ├── Room.cs
│   ├── RoomDescription.cs
│   ├── Item.cs
│   ├── ItemAdjective.cs
│   ├── ItemStates.cs
│   ├── RoomAction.cs
│   ├── GameSave.cs
│   ├── InventoryItem.cs
│   ├── CompletedAction.cs
│   ├── ExaminableObject.cs
│   ├── CompletedExaminableInteraction.cs
│   ├── RevealedExaminableObject.cs
│   ├── ActivatedExaminableObject.cs
│   ├── VisitedRoom.cs
│   └── VocabularyWord.cs
├── Services/            # Business logic services
│   ├── GameStateManager.cs
│   ├── SaveGameService.cs
│   ├── SemanticResolver.cs
│   ├── RoomDescriptionResolver.cs
│   ├── ParsedInput.cs
│   └── DebugLogger.cs
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

### Movement & Exploration
- **look** (l) - Look around the current room and see visible objects
- **examine** (x, inspect) - Examine an object, item, or feature in detail
- **north/south/east/west/up/down** (n/s/e/w/u/d) - Move in a direction

### Inventory Management
- **take** (get, grab, pick) - Pick up an item
- **drop** - Drop an item from your inventory
- **inventory** (i, inv) - Show your inventory

### Item Interaction
- **use** [item] - Use an item from inventory
- **light** [item] - Light an item (e.g., lantern, torch)
- **extinguish** [item] - Extinguish a lit item

### Object Interaction
- **activate** (press, toggle, flip, pull, push, trigger) - Activate a switch, lever, or button
- **action** (do, perform) - Perform special actions in rooms (legacy command)

### Utility
- **help** (?, commands) - Show available commands
- **quit** (exit, q) - Exit the game

### Advanced Features
- **Semantic Matching** - Commands support natural language with fuzzy matching and adjectives (e.g., "examine old brass lantern")
- **Context-Aware** - The game understands what you're trying to do even with typos or varied phrasing
- **Dynamic Compass** - Shows available exits with color coding for visited (grey) vs. unvisited (cyan) rooms

## Adding New Commands

To add a new command, create a class implementing `IGameCommand`:

```csharp
public class MyCustomCommand : IGameCommand
{
    public string Name => "mycommand";
    public string Description => "Does something cool";
    public string[] Aliases => ["mc", "cool"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        // Access parsed command components
        var target = input.DirectObject;  // What the player is targeting
        var adjectives = input.Adjectives; // Descriptive words

        // Your command logic here
        return CommandResult.Ok("Command executed!");
    }
}
```

Then register it in `GameEngine.InitializeCommands()`:

```csharp
_commandRegistry.RegisterCommand(new MyCustomCommand());
```

The `ParsedInput` object provides:
- `Verb` - The command verb (e.g., "examine")
- `DirectObject` - The target noun (e.g., "lantern")
- `Adjectives` - List of descriptive words (e.g., ["brass", "old"])
- `Preposition` - Preposition used (e.g., "with", "on")
- `IndirectObject` - Secondary target (e.g., "examine book with magnifying glass")

## Database Schema

The game uses the following main entities:

### Core Entities
- **Room** - Game locations with navigation links, deadly room mechanics, and win/lose conditions
- **RoomDescription** - Conditional room descriptions based on game state
- **Item** - Objects that can be found or carried with state support (lit/extinguished)
- **ItemAdjective** - Adjectives for semantic item matching
- **ExaminableObject** - Objects that can be examined with keywords, hidden/reveal mechanics, and activation
- **RoomAction** - Special actions available in specific rooms

### Game State Tracking (Per-Save)
- **GameSave** - Saved game state with health, current room, and completion status
- **InventoryItem** - Items in player's inventory with state
- **CompletedAction** - Tracks completed room actions per save
- **CompletedExaminableInteraction** - Tracks completed examinable interactions (unlocking paths)
- **RevealedExaminableObject** - Tracks which hidden objects have been revealed per save
- **ActivatedExaminableObject** - Tracks which switches/levers have been activated per save
- **VisitedRoom** - Tracks which rooms the player has visited per save

### Vocabulary & Parsing
- **VocabularyWord** - Custom vocabulary for semantic command parsing

## The Sample Game

The seeded game includes:

### Locations
- **Mansion Entrance** - Starting location
- **Grand Foyer** - Central hub with a mysterious gargoyle statue and crystal chandelier
- **Library** - Contains clues and a hidden passage to the secret room
- **Kitchen** - Access to the dangerous cellar below
- **Dark Cellar** - Deadly without a light source (dynamic description based on lantern state)
- **Master Bedroom** - Contains the golden key and hidden compartment
- **Secret Room** - The winning location with treasure

### Items to Discover
- **Brass Lantern** - Provides light in dark areas (can be lit/extinguished)
- **Golden Key** - Unlocks the secret passage in the library
- **Ancient Book** - Contains cryptic clues about the mansion
- **Stone Statue** - An immovable gargoyle with hidden secrets

### Interactive Objects
- **Chandelier** - Can be examined (visible in room description)
- **Gargoyle Statue** - Examine to reveal a hidden switch
- **Hidden Switch** - Activate to unlock a compartment in another room
- **Northern Bookshelf** - Examine with the golden key to unlock the secret passage
- **Hidden Safe** - Revealed when you take the ancient book
- **Floor Compartment** - Hidden compartment revealed by activating the switch

### Game Mechanics Featured
- Health system with deadly cellar that damages you without protection
- Item state system (lit lantern protects you in the cellar)
- Hidden object reveals (examining statue reveals switch, taking book reveals safe)
- Cross-room activation (switch in foyer reveals compartment in bedroom)
- Locked passages that require examination with quest items
- Dynamic room descriptions (cellar changes based on whether you have a lit lantern)
- Progressive exploration tracking with compass

## Extending the Game

### Adding New Content

Edit `DatabaseSeeder.cs` to add new content. The seeder runs automatically when you start a new game:

#### Adding Rooms
```csharp
var newRoom = new Room
{
    Name = "Secret Garden",
    Description = "A beautiful overgrown garden...",
    IsStartingRoom = false,
    IsWinningRoom = false,
    IsDeadlyRoom = false
};
```

#### Adding Items with States
```csharp
var torch = new Item
{
    Name = "Torch",
    Description = "A wooden torch",
    IsCollectable = true,
    RoomId = newRoom.Id,
    CanBeLit = true,
    UseMessage = "The torch burns brightly."
};
```

#### Adding Examinable Objects
```csharp
var painting = new ExaminableObject
{
    RoomId = room.Id,
    Name = "painting",
    DisplayName = "Oil Painting",
    Description = "A detailed examination reveals...",
    LookDescription = "An old oil painting hangs on the wall",
    Keywords = "portrait,canvas,frame",
    IsHidden = false,
    ShowInRoomDescription = true
};
```

#### Adding Hidden Objects with Reveals
```csharp
var hiddenNote = new ExaminableObject
{
    Name = "note",
    Description = "A crumpled note with a secret message",
    IsHidden = true,
    RevealedByItemId = book.Id,  // Revealed when book is taken
    RevealMessage = "A note falls out of the book!",
    ShowRevealMessage = true
};
```

#### Adding Activatable Switches
```csharp
var lever = new ExaminableObject
{
    Name = "lever",
    Description = "An old rusty lever",
    IsActivatable = true,
    IsOneTimeUse = true,
    ActivationMessage = "You pull the lever and hear a distant rumble!",
    RevealsExaminableId = secretDoor.Id
};
```

#### Adding Dynamic Room Descriptions
```csharp
var lightDescription = new RoomDescription
{
    RoomId = darkRoom.Id,
    Description = "With light, you can see clearly...",
    Priority = 100,
    ConditionType = DescriptionConditionTypes.ItemState,
    RequiredItemId = lantern.Id,
    RequiredItemState = ItemStates.Lit,
    ItemMustBeOwned = true
};
```

### Custom Game Logic

- **GameStateManager** - Modify for custom state tracking and game mechanics
- **SaveGameService** - Extend for additional save features
- **Commands** - Add new commands for new gameplay mechanics
- **SemanticResolver** - Enhance natural language understanding
- **RoomDescriptionResolver** - Add new condition types for dynamic descriptions

## Technical Details

- **Framework:** .NET 9.0
- **ORM:** Entity Framework Core 9.0
- **Database:** SQLite
- **UI Library:** Spectre.Console 0.52.0
- **Configuration:** YamlDotNet 16.2.1
- **Architecture:** Command Pattern, State Pattern, Repository Pattern

## License

This is a sample project for educational purposes.

## Game Configuration

The `game-config.yaml` file supports the following settings:

```yaml
GameName: Mystery Mansion Adventure
GameDescription: Explore a mysterious mansion and uncover its secrets
Author: Adventure Engine
Version: 1.0.0
StartingHealth: 100           # Player's initial health
MaxInventoryWeight: 100       # Maximum inventory capacity
MaxSaveSlots: 5               # Number of save slots available
DatabasePath: adventure.db    # SQLite database file path

UI:
  TitleColor: cyan            # Color for titles and headers
  DescriptionColor: white     # Color for descriptions
  ErrorColor: red             # Color for error messages
  SuccessColor: green         # Color for success messages
  WarningColor: yellow        # Color for warnings
  PromptColor: blue           # Color for input prompt
```

## Key Systems Explained

### Health System
- Players start with configurable health (default: 100)
- Deadly rooms can damage the player when entered
- Protective items can prevent damage (e.g., lit lantern in dark cellar)
- Game ends when health reaches zero

### Hidden Object Reveal System
Objects can be revealed through three mechanisms:
1. **Examining another object** - Set `RevealedByExaminableId`
2. **Picking up an item** - Set `RevealedByItemId`
3. **Activating a switch** - Set `RevealsExaminableId` on the activatable object

### Activation System
- Objects can be marked as `IsActivatable = true`
- Use `IsOneTimeUse = true` for switches that only work once
- Activations are tracked per save file
- Can trigger reveals in other rooms

### Dynamic Room Descriptions
Room descriptions can change based on:
- Item ownership and state (e.g., having a lit lantern)
- Completed actions
- Game progression
- Higher priority descriptions override lower priority ones

### Semantic Matching
The game uses fuzzy string matching with:
- Levenshtein distance for typo tolerance
- Adjective matching (e.g., "brass lantern" matches lantern with "brass" adjective)
- Keyword matching for examinable objects
- Context-aware resolution (searches current room first)

## Future Enhancements

Potential features to add:
- NPCs (Non-player characters) with dialogue trees
- Combat system with weapons and armor
- Advanced puzzle mechanics (combination locks, riddles)
- Time-based events and day/night cycles
- Achievements system
- Sound effects and music
- Custom scripting language for room events
- Weather and environmental effects
- Skill system (lockpicking, persuasion, etc.)
- Random item/enemy placement for replayability
