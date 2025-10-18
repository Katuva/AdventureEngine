using AdventureEngine.Models;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Data;

/// <summary>
/// Seeds the database with initial game content
/// </summary>
public class DatabaseSeeder(AdventureDbContext context)
{
    public async Task SeedAsync()
    {
        // Check if already seeded
        if (await context.Rooms.AnyAsync())
        {
            return; // Database already has data
        }

        // Create rooms
        var entrance = new Room
        {
            Name = "Mansion Entrance",
            Description = "You stand before a grand Victorian mansion. The heavy oak doors creak as they swing open, revealing a dimly lit foyer. Dust motes dance in the pale moonlight streaming through grimy windows.",
            IsStartingRoom = true
        };

        var foyer = new Room
        {
            Name = "Grand Foyer",
            Description = "A magnificent chandelier hangs precariously from the high ceiling. A grand staircase curves upward to the second floor. Portraits of stern-faced ancestors line the walls, their eyes seeming to follow your every move. A stone gargoyle statue stands in the corner."
        };

        var library = new Room
        {
            Name = "Library",
            Description = "Floor-to-ceiling bookshelves line every wall. The smell of old leather and aged paper fills the air. A large mahogany desk sits in the center, covered in scattered documents. The northern bookshelf seems particularly old and ornate."
        };

        var kitchen = new Room
        {
            Name = "Kitchen",
            Description = "An old-fashioned kitchen with a massive wood-burning stove. Copper pots and pans hang from hooks on the wall. A door in the back leads to what appears to be a cellar."
        };

        var cellar = new Room
        {
            Name = "Dark Cellar",
            Description = "The cellar is pitch black and smells of mold and decay. You can barely see your hand in front of your face. Strange sounds echo from the darkness.",
            IsDeadlyRoom = true,
            DamageAmount = 30,
            DeathMessage = "Without a light source, you stumble in the darkness and hurt yourself."
        };

        var bedroom = new Room
        {
            Name = "Master Bedroom",
            Description = "A lavish bedroom with a four-poster bed draped in moth-eaten velvet. A wardrobe stands against one wall, and a writing desk sits by the window."
        };

        var secretRoom = new Room
        {
            Name = "Secret Room",
            Description = "Behind a hidden door, you discover a small room filled with treasure! Gold coins, jewels, and ancient artifacts glitter in the light. You've found the mansion's secret!",
            IsWinningRoom = true,
            WinMessage = "Congratulations! You've discovered the hidden treasure of Mystery Mansion!"
        };

        context.Rooms.AddRange(entrance, foyer, library, kitchen, cellar, bedroom, secretRoom);
        await context.SaveChangesAsync();

        // Set up room connections (need IDs first)
        entrance.SouthRoomId = foyer.Id;
        foyer.NorthRoomId = entrance.Id;
        foyer.EastRoomId = library.Id;
        foyer.WestRoomId = kitchen.Id;
        foyer.UpRoomId = bedroom.Id;
        library.WestRoomId = foyer.Id;
        kitchen.EastRoomId = foyer.Id;
        kitchen.DownRoomId = cellar.Id;
        cellar.UpRoomId = kitchen.Id;
        bedroom.DownRoomId = foyer.Id;

        // Create items
        var lantern = new Item
        {
            Name = "Lantern",
            Description = "An old brass lantern. It still has oil and works perfectly.",
            IsCollectable = true,
            RoomId = library.Id,
            UseMessage = "The lantern illuminates the darkness around you."
        };

        var key = new Item
        {
            Name = "Golden Key",
            Description = "An ornate golden key with mysterious engravings.",
            IsCollectable = true,
            IsQuestItem = true,
            RoomId = bedroom.Id
        };

        var book = new Item
        {
            Name = "Ancient Book",
            Description = "A leather-bound book titled 'Secrets of the Mansion'. It contains cryptic clues.",
            IsCollectable = true,
            RoomId = library.Id,
            UseMessage = "The book mentions a hidden door behind the bookshelf in the library..."
        };

        var statue = new Item
        {
            Name = "Stone Statue",
            Description = "A heavy stone statue of a gargoyle. Too heavy to carry.",
            IsCollectable = false,
            RoomId = foyer.Id
        };

        context.Items.AddRange(lantern, key, book, statue);
        await context.SaveChangesAsync();

        // Create room actions
        var enterCellarSafely = new RoomAction
        {
            RoomId = cellar.Id,
            ActionName = "use lantern",
            Description = "Light your way with the lantern",
            RequiredItemId = lantern.Id,
            SuccessMessage = "The lantern's warm glow reveals the cellar is just an empty storage room. Nothing interesting here.",
            FailureMessage = "It's too dark to see anything!",
            IsRepeatable = true
        };

        var readBook = new RoomAction
        {
            RoomId = library.Id,
            ActionName = "read book",
            Description = "Read the ancient book carefully",
            RequiredItemId = book.Id,
            SuccessMessage = "The book reveals: 'The golden key lies where dreams are made. The secret door awaits those who possess it.'",
            FailureMessage = "You don't have the book to read.",
            IsRepeatable = true
        };

        context.RoomActions.AddRange(enterCellarSafely, readBook);
        await context.SaveChangesAsync();

        // Update secret room connection (after action is created)
        library.UpRoomId = null; // Will be unlocked by action
        secretRoom.DownRoomId = library.Id;

        // Create examinable objects
        var statueExamine = new ExaminableObject
        {
            RoomId = foyer.Id,
            Name = "statue",
            Description = "The gargoyle statue is intricately carved from dark stone. Its eyes seem to follow you around the room. You notice strange symbols etched into its base.",
            Keywords = "gargoyle,symbols,base",
            IsHidden = false
        };

        var bookExamine = new ExaminableObject
        {
            RoomId = library.Id,
            Name = "book",
            Description = "The ancient book's cover reads 'Secrets of the Mansion'. Opening it carefully, you read: 'The golden key lies where dreams are made. The secret door awaits those who possess it, hidden behind the northern bookshelf.'",
            Keywords = "ancient book,tome,secrets",
            IsHidden = false
        };

        var trapdoorOutline = new ExaminableObject
        {
            RoomId = library.Id,
            Name = "bookshelf",
            Description = "Upon closer inspection of the northern bookshelf, you notice a faint outline behind it. It appears to be concealing a hidden passage! Perhaps with the right key, you could unlock it.",
            Keywords = "outline,hidden passage,secret door,passage,door,northern bookshelf",
            IsHidden = false,
            RequiredItemId = key.Id,
            UnlocksRoomId = secretRoom.Id,
            SuccessMessage = "You insert the golden key into a hidden keyhole behind the bookshelf. With a soft click, the entire bookshelf swings open, revealing a secret passage leading upward!",
            FailureMessage = "The bookshelf doesn't budge. Perhaps you need something to unlock it?"
        };

        var chandelier = new ExaminableObject
        {
            RoomId = foyer.Id,
            Name = "chandelier",
            Description = "The magnificent crystal chandelier hangs precariously from rusty chains. Dust covers most of the crystals, but they still catch what little light filters through the windows.",
            Keywords = "crystal,chains,crystals",
            IsHidden = false
        };

        context.ExaminableObjects.AddRange(statueExamine, bookExamine, trapdoorOutline, chandelier);

        await context.SaveChangesAsync();
    }
}
