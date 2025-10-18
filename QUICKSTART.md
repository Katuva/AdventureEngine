# Quick Start Guide

## Installation and First Run

### 1. Prerequisites

Make sure you have .NET 9.0 SDK installed:

```bash
dotnet --version
```

If not installed, download from: https://dotnet.microsoft.com/download

### 2. Build the Project

```bash
cd AdventureEngine
dotnet restore
dotnet build
```

### 3. Create Database Migration

```bash
dotnet ef migrations add InitialCreate
```

This creates the migration files in a `Migrations/` folder.

### 4. Run the Game

```bash
dotnet run
```

On first run, the application will:
1. Create `game-config.yaml` with default settings
2. Create and migrate the SQLite database (`adventure.db`)
3. Seed the database with the sample game content
4. Display the main menu

## Playing the Game

### Main Menu

You'll see options to:
- **New Game** - Create a new save slot
- **Load Game** - Continue from an existing save
- **Delete Save** - Remove a save slot
- **Exit** - Quit the application

### Game Commands

Once in-game, use these commands:

**Navigation:**
- `north` (or `n`) - Move north
- `south` (or `s`) - Move south
- `east` (or `e`) - Move east
- `west` (or `w`) - Move west
- `up` (or `u`) - Move up
- `down` (or `d`) - Move down

**Interaction:**
- `look` (or `l`) - Examine your surroundings
- `take <item>` - Pick up an item
- `inventory` (or `i`) - Check your inventory
- `use <item>` - Use an item you're carrying
- `action <name>` - Perform a special action (type just `action` to see available actions)

**System:**
- `help` (or `?`) - Show all commands
- `quit` (or `q`) - Exit to main menu (auto-saves)

## The Sample Game - Mystery Mansion

### Goal

Find the hidden treasure room and win the game!

### Starting Guide

1. Start at the **Mansion Entrance**
2. Use `look` to examine your surroundings
3. Navigate through rooms using directional commands
4. Pick up useful items (especially in the **Library** and **Master Bedroom**)
5. Read the **Ancient Book** for clues
6. Beware of the **Dark Cellar** - you'll need light!
7. Find the **Golden Key** to unlock the secret
8. Discover the **Secret Room** to win

### Tips

- Use `look` frequently to find items and exits
- The `inventory` command shows what you're carrying
- Some actions require specific items
- Not all rooms are safe without the right equipment
- Keep track of what the book tells you

## Sample Playthrough

```
> look
You stand before a grand Victorian mansion...
Exits: south

> south
You move south to the Grand Foyer.
A magnificent chandelier hangs...

> east
You move east to the Library.
Floor-to-ceiling bookshelves...

> look
...
You can see:
  - Lantern: An old brass lantern...
  - Ancient Book: A leather-bound book...
Exits: west

> take lantern
You take the Lantern.

> take book
You take the Ancient Book.

> use book
The book reveals: 'The golden key lies where dreams are made...'

> west
You move west to the Grand Foyer.

> up
You move up to the Master Bedroom.
A lavish bedroom...

> look
...
You can see:
  - Golden Key: An ornate golden key...

> take key
You take the Golden Key.

> down
You move down to the Grand Foyer.

> east
You move east to the Library.

> action
Available actions:
  - unlock secret: Use the golden key to unlock the hidden door

> action unlock secret
The key fits perfectly! A section of the bookshelf swings open...

> up
You move up to the Secret Room.

VICTORY!
Congratulations! You've discovered the hidden treasure of Mystery Mansion!
```

## Troubleshooting

### "No .NET SDKs were found"

Install the .NET 9.0 SDK from https://dotnet.microsoft.com/download

### "Could not execute because the application was not found"

Make sure you're in the correct directory:
```bash
cd AdventureEngine/AdventureEngine
dotnet run
```

### Database Errors

Reset the database:
```bash
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet run
```

### "Package not found" Errors

Restore packages:
```bash
dotnet restore
dotnet build
```

## Next Steps

- Read the [README.md](README.md) for full feature documentation
- Check out [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) to learn how to extend the game
- Create your own rooms, items, and actions by modifying `DatabaseSeeder.cs`
- Add new commands by implementing `IGameCommand`
- Customize the game appearance in `game-config.yaml`

## Having Fun?

Try:
- Creating your own adventure by modifying the seeder
- Adding new command types
- Implementing a combat system
- Adding NPCs to talk to
- Creating puzzles and quest chains

Enjoy your adventure!
