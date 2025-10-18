using System.ComponentModel;
using AdventureEngine.Config;
using AdventureEngine.Data;
using AdventureEngine.Game;
using AdventureEngine.Services;
using AdventureEngine.UI;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AdventureEngine.Commands;

/// <summary>
/// CLI command for running the Adventure Engine game
/// </summary>
public class PlayCommand : AsyncCommand<PlayCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-d|--debug")]
        [Description("Enable debug output")]
        public bool Debug { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            // Set debug mode
            DebugLogger.IsEnabled = settings.Debug;

            if (settings.Debug)
            {
                AnsiConsole.MarkupLine("[yellow]Debug mode enabled[/]");
            }

            // Load configuration
            var config = ConfigurationLoader.LoadConfiguration();

            // Set up database
            var optionsBuilder = new DbContextOptionsBuilder<AdventureDbContext>();
            optionsBuilder.UseSqlite($"Data Source={config.DatabasePath}");

            await using var dbContext = new AdventureDbContext(optionsBuilder.Options);

            // Ensure database is created and migrated
            await dbContext.Database.MigrateAsync();

            // Seed initial data if needed
            var seeder = new DatabaseSeeder(dbContext);
            await seeder.SeedAsync();

            // Seed vocabulary data
            var vocabSeeder = new VocabularySeeder(dbContext);
            await vocabSeeder.SeedAsync();

            // Initialize services
            var ui = new ConsoleUI(config);
            var saveService = new SaveGameService(dbContext, config);
            var mainMenu = new MainMenu(ui, saveService);
            var gameEngine = new GameEngine(dbContext, ui, config);

            // Main application loop
            var running = true;
            while (running)
            {
                var action = await mainMenu.ShowAsync();

                switch (action.Type)
                {
                    case MenuActionType.StartGame:
                        await gameEngine.RunAsync(action.SaveGameId);
                        break;

                    case MenuActionType.ShowMenu:
                        // Loop continues to show menu again
                        break;

                    case MenuActionType.Exit:
                        running = false;
                        break;
                }
            }

            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[cyan1]Thank you for playing![/]");

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return 1;
        }
    }
}
