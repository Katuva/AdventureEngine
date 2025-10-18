# Developer Guide - Adventure Engine

## Quick Start for Development

### Running the Application

```bash
# Restore dependencies
dotnet restore

# Create initial migration
dotnet ef migrations add InitialCreate

# Run the game
dotnet run
```

### Building for Release

```bash
dotnet build -c Release
dotnet publish -c Release
```

## Architecture Overview

### Design Patterns Used

1. **Command Pattern** - All player actions (move, take, look, etc.)
2. **State Pattern** - Game state management
3. **Repository Pattern** - Entity Framework DbContext
4. **Factory Pattern** - DbContextFactory for design-time operations
5. **Service Layer Pattern** - Business logic separation

### Key Components

#### 1. Game Engine (`Game/GameEngine.cs`)

The main orchestrator that:
- Initializes the command registry
- Runs the game loop
- Processes player input
- Handles win/lose conditions

#### 2. Command System (`Game/Actions/`)

All commands implement `IGameCommand`:

```csharp
public interface IGameCommand
{
    string Name { get; }
    string Description { get; }
    string[] Aliases { get; }
    Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args);
}
```

**Key Features:**
- Easy to add new commands
- Consistent error handling
- Support for aliases
- Async execution

#### 3. Game State Manager (`Services/GameStateManager.cs`)

Manages:
- Current player position
- Inventory management
- Action completion tracking
- Room navigation
- Win/lose state

#### 4. Save System (`Services/SaveGameService.cs`)

Handles:
- Multiple save slots
- Load/save operations
- Save slot management

#### 5. UI Layer (`UI/`)

**ConsoleUI.cs:**
- Wraps Spectre.Console functionality
- Provides consistent styling
- Handles all visual output

**MainMenu.cs:**
- Main menu navigation
- Save slot selection
- New game creation

## Adding New Features

### Adding a New Command

1. Create a new class in `Game/Actions/`:

```csharp
public class DropCommand : IGameCommand
{
    public string Name => "drop";
    public string Description => "Drop an item from inventory";
    public string[] Aliases => ["discard", "leave"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Error("Drop what?");

        var itemName = string.Join(" ", args);
        // Implementation here
        return CommandResult.Ok($"You drop the {itemName}.");
    }
}
```

2. Register in `GameEngine.InitializeCommands()`:

```csharp
_commandRegistry.RegisterCommand(new DropCommand());
```

### Adding New Database Models

1. Create model in `Models/`:

```csharp
public class NPC
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
}
```

2. Add DbSet to `AdventureDbContext`:

```csharp
public DbSet<NPC> NPCs => Set<NPC>();
```

3. Configure relationships in `OnModelCreating()`:

```csharp
modelBuilder.Entity<NPC>()
    .HasOne(n => n.Room)
    .WithMany()
    .HasForeignKey(n => n.RoomId);
```

4. Create migration:

```bash
dotnet ef migrations add AddNPCs
```

### Extending the Seeder

Edit `Data/DatabaseSeeder.cs`:

```csharp
public async Task SeedAsync()
{
    if (await _context.Rooms.AnyAsync())
        return;

    // Create your rooms
    var room1 = new Room { ... };
    var room2 = new Room { ... };

    _context.Rooms.AddRange(room1, room2);
    await _context.SaveChangesAsync();

    // Set up connections
    room1.NorthRoomId = room2.Id;
    room2.SouthRoomId = room1.Id;

    // Create items
    var sword = new Item { ... };
    _context.Items.Add(sword);

    await _context.SaveChangesAsync();
}
```

### Custom Game Logic

#### Example: Adding Health System

1. Add to `GameSave` model:

```csharp
public int Health { get; set; } = 100;
```

2. Create migration:

```bash
dotnet ef migrations add AddHealthSystem
```

3. Add methods to `GameStateManager`:

```csharp
public async Task DamagePlayerAsync(int damage)
{
    var save = await Context.GameSaves.FindAsync(CurrentSaveId);
    if (save != null)
    {
        save.Health -= damage;
        if (save.Health <= 0)
        {
            save.IsPlayerDead = true;
        }
        await Context.SaveChangesAsync();
    }
}

public async Task<int> GetPlayerHealthAsync()
{
    var save = await Context.GameSaves.FindAsync(CurrentSaveId);
    return save?.Health ?? 0;
}
```

4. Create new command:

```csharp
public class StatsCommand : IGameCommand
{
    public string Name => "stats";
    public string Description => "Show your statistics";
    public string[] Aliases => ["status"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args)
    {
        var health = await gameState.GetPlayerHealthAsync();
        return CommandResult.Ok($"Health: {health}/100");
    }
}
```

## Database Migrations

### Common Commands

```bash
# Add new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script

# Drop database (careful!)
dotnet ef database drop
```

### Migration Best Practices

1. Always test migrations before committing
2. Use descriptive migration names
3. Review generated migration code
4. Never modify applied migrations
5. Keep data migrations separate from schema migrations

## Testing

### Manual Testing Checklist

- [ ] Create new game save
- [ ] Load existing save
- [ ] Delete save
- [ ] Navigate between rooms
- [ ] Pick up items
- [ ] Use items
- [ ] Perform room actions
- [ ] Win condition
- [ ] Lose condition
- [ ] Quit and auto-save
- [ ] Invalid commands

### Future: Unit Tests

```csharp
[Fact]
public async Task TakeCommand_ItemExists_AddsToInventory()
{
    // Arrange
    var command = new TakeCommand();
    var gameState = CreateTestGameState();

    // Act
    var result = await command.ExecuteAsync(gameState, new[] { "key" });

    // Assert
    Assert.True(result.Success);
    Assert.True(await gameState.HasItemAsync(testItemId));
}
```

## Performance Considerations

### Database Queries

- Use `.Include()` for eager loading related entities
- Avoid N+1 queries
- Use `.AsNoTracking()` for read-only queries
- Index frequently queried fields

### Memory Management

- Dispose DbContext properly (using statements)
- Don't hold references to large collections
- Cache frequently accessed data

## Troubleshooting

### Common Issues

**Issue:** "No migrations found"
```bash
dotnet ef migrations add InitialCreate
```

**Issue:** "Database already exists"
```bash
dotnet ef database drop
dotnet ef database update
```

**Issue:** "Package not found"
```bash
dotnet restore
dotnet build
```

**Issue:** "Cannot find room/item"
- Check that `DatabaseSeeder.SeedAsync()` ran
- Verify database file exists
- Check for seeding exceptions

## Code Style

### Naming Conventions

- Classes: `PascalCase`
- Methods: `PascalCase`
- Properties: `PascalCase`
- Private fields: `_camelCase`
- Parameters: `camelCase`
- Constants: `PascalCase`

### Organization

- One class per file
- Files named after the class
- Group related functionality in folders
- Keep classes focused (Single Responsibility Principle)

## Resources

- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Spectre.Console Documentation](https://spectreconsole.net/)
- [YamlDotNet Documentation](https://github.com/aaubry/YamlDotNet)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)

## Contributing

When adding new features:

1. Follow existing code patterns
2. Add XML documentation comments
3. Update README.md with new features
4. Test thoroughly
5. Consider backwards compatibility

## Example: Complete Feature Addition

Let's add an NPC conversation system:

### 1. Create Model

```csharp
// Models/NPC.cs
public class NPC
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Greeting { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
}
```

### 2. Update DbContext

```csharp
public DbSet<NPC> NPCs => Set<NPC>();

// In OnModelCreating:
modelBuilder.Entity<NPC>()
    .HasOne(n => n.Room)
    .WithMany()
    .HasForeignKey(n => n.RoomId);
```

### 3. Create Migration

```bash
dotnet ef migrations add AddNPCs
```

### 4. Update Seeder

```csharp
var oldMan = new NPC
{
    Name = "Old Man",
    Greeting = "Well hello there, young adventurer!",
    RoomId = library.Id
};
_context.NPCs.Add(oldMan);
await _context.SaveChangesAsync();
```

### 5. Create Command

```csharp
public class TalkCommand : IGameCommand
{
    public string Name => "talk";
    public string Description => "Talk to an NPC";
    public string[] Aliases => ["speak", "chat"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args)
    {
        var room = await gameState.GetCurrentRoomAsync();
        var npcs = await gameState.Context.NPCs
            .Where(n => n.RoomId == room!.Id)
            .ToListAsync();

        if (!npcs.Any())
            return CommandResult.Error("There's nobody here to talk to.");

        var npc = npcs.First();
        return CommandResult.Ok($"{npc.Name}: \"{npc.Greeting}\"");
    }
}
```

### 6. Register Command

```csharp
_commandRegistry.RegisterCommand(new TalkCommand());
```

### 7. Update Look Command

```csharp
// Show NPCs when looking around
var npcs = await gameState.Context.NPCs
    .Where(n => n.RoomId == room.Id)
    .ToListAsync();

if (npcs.Any())
{
    description += "\n\nYou see:";
    foreach (var npc in npcs)
    {
        description += $"\n  - {npc.Name}";
    }
}
```

Done! You now have a working NPC conversation system.
