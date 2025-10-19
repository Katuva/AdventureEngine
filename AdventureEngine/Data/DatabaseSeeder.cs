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
            Description = "A cellar beneath the kitchen.",  // Fallback only - conditional descriptions are used
            IsDeadlyRoom = true,
            DamageAmount = 30,
            DeathMessage = "Without a light source, you stumble in the darkness and hurt yourself.",
            IsDark = true  // Requires light source to see
            // ProtectionItemId and LightSourceItemId will be set after lantern is created
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

        // Healing items
        var healthPotion = new Item
        {
            Name = "Health Potion",
            Description = "A small glass vial filled with a shimmering red liquid. The label reads 'Restores vitality'.",
            IsCollectable = true,
            RoomId = kitchen.Id,
            HealingAmount = 30,
            MaxUses = 1, // Single use
            UseMessage = "You drink the health potion. The warm liquid courses through your veins, mending your wounds.",
            EmptyDescription = "The vial is empty.",
            DisappearsWhenEmpty = false // Does not disappear after use
        };

        var bandages = new Item
        {
            Name = "Bandages",
            Description = "A roll of clean white bandages. Good for treating minor wounds.",
            IsCollectable = true,
            RoomId = bedroom.Id,
            HealingAmount = 15,
            MaxUses = 3, // Can be used 3 times
            UseMessage = "You apply the bandages to your wounds.",
            EmptyDescription = "The bandages have all been used up.",
            DisappearsWhenEmpty = true // Disappears when all used
        };

        var infinitePotion = new Item
        {
            Name = "Eternal Elixir",
            Description = "A mystical crystal vial filled with an endless supply of shimmering red liquid with swirls of gold. The potion seems to refill itself magically.",
            IsCollectable = true,
            RoomId = bedroom.Id,
            HealingAmount = 25,
            MaxUses = 0, // Infinite uses!
            UseMessage = "You drink from the eternal elixir. The magical liquid heals your wounds, and the vial refills itself instantly!",
            DisappearsWhenEmpty = false // Never depletes, so never disappears
        };

        context.Items.AddRange(lantern, key, book, statue, healthPotion, bandages, infinitePotion);
        await context.SaveChangesAsync();

        // Set cellar protection and light source: requires lit lantern
        cellar.ProtectionItemId = lantern.Id;
        cellar.LightSourceItemId = lantern.Id;
        cellar.RequiredItemState = ItemStates.Lit;  // Must be lit, not just carried
        await context.SaveChangesAsync();

        // Create room actions
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

        context.RoomActions.Add(readBook);
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
            IsHidden = false,
            ShowInRoomDescription = false  // Not shown in look - must examine specifically
        };

        var bookExamine = new ExaminableObject
        {
            RoomId = library.Id,
            Name = "book",
            Description = "The ancient book's cover reads 'Secrets of the Mansion'. Opening it carefully, you read: 'The golden key lies where dreams are made. Those keen of eye will find the hidden door.'",
            Keywords = "ancient book,tome,secrets",
            IsHidden = false,
            ShowInRoomDescription = false  // Not shown in look - must examine specifically
        };

        var trapdoorOutline = new ExaminableObject
        {
            RoomId = library.Id,
            Name = "bookshelf",
            Description = "Upon closer inspection of the northern bookshelf, you notice a faint outline behind it. It appears to be concealing a hidden passage! Perhaps with the right key, you could unlock it.",
            Keywords = "outline,hidden passage,secret door,passage,door,northern bookshelf,bookcase",
            IsHidden = false,
            ShowInRoomDescription = false,  // Not shown in look - must examine specifically
            RequiredItemId = key.Id,
            UnlocksRoomId = secretRoom.Id,
            UnlockDirection = "up",
            SuccessMessage = "You insert the golden key into a hidden keyhole behind the bookshelf. With a soft click, the entire bookshelf swings open, revealing a secret passage leading upward!",
            FailureMessage = "The bookshelf doesn't budge. Perhaps you need something to unlock it?"
        };

        var chandelier = new ExaminableObject
        {
            RoomId = foyer.Id,
            Name = "chandelier",
            DisplayName = "Chandelier",
            Description = "The magnificent crystal chandelier hangs precariously from rusty chains. Dust covers most of the crystals, but they still catch what little light filters through the windows.",
            LookDescription = "A magnificent crystal chandelier hangs overhead",
            Keywords = "crystal,chains,crystals",
            IsHidden = false,
            ShowInRoomDescription = true  // Shows in look - obvious feature of the room
        };

        context.ExaminableObjects.AddRange(statueExamine, bookExamine, trapdoorOutline, chandelier);
        await context.SaveChangesAsync();

        // Create hidden examinable objects (revealed by triggers)
        // First, create the object that will be revealed by the switch
        var hiddenCompartment = new ExaminableObject
        {
            RoomId = bedroom.Id,
            Name = "compartment",
            DisplayName = "Floor Compartment",
            Description = "A hidden compartment in the bedroom floor has opened! Inside, you see a glint of something valuable.",
            LookDescription = "An open compartment in the floor",
            Keywords = "hidden compartment,floor panel,secret compartment,panel",
            IsHidden = true,
            ShowInRoomDescription = true,  // When revealed, shows in look - it's an obvious opening
            RevealMessage = "With a grinding sound, a hidden compartment in the bedroom floor slides open!",
            ShowRevealMessage = false  // Don't show message - switch is in different room
        };

        context.ExaminableObjects.Add(hiddenCompartment);
        await context.SaveChangesAsync();

        // Hidden switch revealed by examining the statue - activating it reveals the compartment
        var hiddenSwitch = new ExaminableObject
        {
            RoomId = foyer.Id,
            Name = "switch",
            Description = "A small, ornate switch hidden in the statue's base. It appears to be connected to some mechanism in the walls.",
            Keywords = "lever,button,mechanism,hidden switch",
            IsHidden = true,
            ShowInRoomDescription = false,  // Even when revealed, not shown in look - part of statue
            RevealedByExaminableId = statueExamine.Id,
            RevealMessage = "As you examine the statue closely, you notice a small switch cleverly concealed among the carved symbols on its base!",
            IsActivatable = true,
            MaxUses = 1,
            ActivationMessage = "You press the hidden switch. You hear a distant grinding sound echo through the mansion!",
            RevealsExaminableId = hiddenCompartment.Id
        };

        // Hidden safe revealed by picking up the ancient book
        var hiddenSafe = new ExaminableObject
        {
            RoomId = library.Id,
            Name = "safe",
            DisplayName = "Hidden Safe",
            Description = "Behind where the book was sitting, you discover a small hidden safe built into the desk. It's locked, but doesn't appear to need a key - just the right combination.",
            LookDescription = "A small safe built into the desk",
            Keywords = "strongbox,lockbox,vault,hidden safe",
            IsHidden = true,
            ShowInRoomDescription = true,  // When revealed, shows in look - it's now obvious
            RevealedByItemId = book.Id,
            RevealMessage = "Lifting the ancient book reveals a hidden safe built into the desk beneath it!",
            ShowRevealMessage = true  // Show message - player is in same room when taking book
        };

        // Healing objects
        var healingFountain = new ExaminableObject
        {
            RoomId = entrance.Id,
            Name = "fountain",
            DisplayName = "Healing Fountain",
            Description = "An ancient stone fountain with crystal-clear water. A plaque reads: 'Waters of restoration, limited in power'.",
            LookDescription = "A small stone fountain with shimmering water",
            Keywords = "water,basin,well,healing fountain",
            IsHidden = false,
            ShowInRoomDescription = true,
            IsActivatable = true,
            HealingAmount = 20,
            MaxUses = 5, // Limited uses
            ActivationMessage = "You cup your hands and drink from the fountain. The cool water refreshes you.",
            EmptyDescription = "The fountain has run dry. No more water flows from it."
        };

        var magicalHerbs = new ExaminableObject
        {
            RoomId = cellar.Id,
            Name = "herbs",
            DisplayName = "Magical Herbs",
            Description = "A small patch of glowing herbs growing in a crack in the wall. They emit a faint, soothing aura.",
            LookDescription = "Glowing herbs growing from a crack",
            Keywords = "plants,herb,moss,magical herbs,glowing plants",
            IsHidden = false,
            ShowInRoomDescription = true,
            IsActivatable = true,
            HealingAmount = 50,
            MaxUses = 1,
            ActivationMessage = "You carefully pluck the herbs and consume them. Warmth spreads through your body as your wounds mend.",
            EmptyDescription = "The herbs have been harvested. Only wilted stems remain."
        };

        context.ExaminableObjects.AddRange(hiddenSwitch, hiddenSafe, healingFountain, magicalHerbs);
        await context.SaveChangesAsync();

        // Create treasure item
        var treasure = new Item
        {
            Name = "Golden Amulet",
            Description = "An exquisite golden amulet encrusted with precious gems. It glows with an otherworldly light.",
            IsCollectable = true,
            IsQuestItem = true
        };

        context.Items.Add(treasure);
        await context.SaveChangesAsync();

        // Create treasure chest in the hidden compartment
        var treasureChest = new Container
        {
            RoomId = bedroom.Id,
            Name = "chest",
            DisplayName = "Ornate Chest",
            Description = "A beautifully crafted wooden chest with intricate carvings of mythical creatures. The wood is dark and polished to a deep shine.",
            Keywords = "box,treasure chest,ornate chest",
            EmptyDescription = "nothing but dust",
            StartsOpen = false,
            IsLockable = false,
            ShowInRoomDescription = true,  // Shows when revealed
            IsHidden = true,  // Starts hidden
            RevealedByExaminableId = hiddenCompartment.Id,  // Revealed when compartment is examined
            RevealMessage = "Inside the hidden compartment, you spot an ornate chest!"
        };

        context.Containers.Add(treasureChest);
        await context.SaveChangesAsync();

        // Put treasure inside the chest
        var chestItem = new ContainerItem
        {
            ContainerId = treasureChest.Id,
            ItemId = treasure.Id
        };

        context.ContainerItems.Add(chestItem);
        await context.SaveChangesAsync();

        // Create conditional room descriptions
        // Cellar with lit lantern (high priority)
        var cellarLitDescription = new RoomDescription
        {
            RoomId = cellar.Id,
            Description = "The lantern's warm glow reveals a stone cellar with dusty shelves lining the walls. Old wine bottles and forgotten storage crates fill the space. It's musty but harmless.",
            Priority = 100,
            ConditionType = DescriptionConditionTypes.ItemState,
            RequiredItemId = lantern.Id,
            RequiredItemState = ItemStates.Lit,
            ItemMustBeOwned = true
        };

        // Cellar without lit lantern (default - lower priority)
        var cellarDarkDescription = new RoomDescription
        {
            RoomId = cellar.Id,
            Description = "The cellar is pitch black and smells of mold and decay. You can barely see your hand in front of your face. Strange sounds echo from the darkness. Without a light source, every step is treacherous.",
            Priority = 0,
            ConditionType = DescriptionConditionTypes.Default
        };

        context.RoomDescriptions.AddRange(cellarLitDescription, cellarDarkDescription);
        await context.SaveChangesAsync();

        // Create item adjectives for better semantic matching
        var itemAdjectives = new List<ItemAdjective>
        {
            // Lantern adjectives
            new() { ItemId = lantern.Id, Adjective = "brass", Priority = 2 },
            new() { ItemId = lantern.Id, Adjective = "old", Priority = 1 },

            // Key adjectives
            new() { ItemId = key.Id, Adjective = "golden", Priority = 3 },
            new() { ItemId = key.Id, Adjective = "ornate", Priority = 2 },

            // Book adjectives
            new() { ItemId = book.Id, Adjective = "ancient", Priority = 2 },
            new() { ItemId = book.Id, Adjective = "leather", Priority = 1 },

            // Statue adjectives
            new() { ItemId = statue.Id, Adjective = "stone", Priority = 2 },
            new() { ItemId = statue.Id, Adjective = "heavy", Priority = 1 },

            // Health potion adjectives
            new() { ItemId = healthPotion.Id, Adjective = "health", Priority = 3 },
            new() { ItemId = healthPotion.Id, Adjective = "red", Priority = 2 },
            new() { ItemId = healthPotion.Id, Adjective = "healing", Priority = 2 },

            // Bandages adjectives
            new() { ItemId = bandages.Id, Adjective = "white", Priority = 1 },
            new() { ItemId = bandages.Id, Adjective = "clean", Priority = 1 }
        };

        context.ItemAdjectives.AddRange(itemAdjectives);
        await context.SaveChangesAsync();
    }
}
