using Spectre.Console;
using AdventureEngine.Services;
using AdventureEngine.Models;

namespace AdventureEngine.UI;

/// <summary>
/// Main menu interface
/// </summary>
public class MainMenu(ConsoleUI ui, SaveGameService saveService)
{
    public async Task<MainMenuAction> ShowAsync()
    {
        ui.ShowIntro();

        var hasSaves = await saveService.HasSavesAsync();

        var choices = new List<string>
        {
            "New Game"
        };

        if (hasSaves)
        {
            choices.Add("Load Game");
            choices.Add("Delete Save");
        }

        choices.Add("Exit");

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan1]What would you like to do?[/]")
                .AddChoices(choices)
        );

        return choice switch
        {
            "New Game" => await HandleNewGameAsync(),
            "Load Game" => await HandleLoadGameAsync(),
            "Delete Save" => await HandleDeleteSaveAsync(),
            "Exit" => MainMenuAction.Exit(),
            _ => MainMenuAction.Exit()
        };
    }

    private async Task<MainMenuAction> HandleNewGameAsync()
    {
        var slotName = AnsiConsole.Ask<string>("[cyan1]Enter a name for your save:[/]");

        try
        {
            var newSave = await saveService.CreateNewGameAsync(slotName);
            return MainMenuAction.StartGame(newSave.Id);
        }
        catch (InvalidOperationException ex)
        {
            ui.ShowError(ex.Message);
            AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
            Console.ReadKey(true);
            return MainMenuAction.ShowMenu();
        }
    }

    private async Task<MainMenuAction> HandleLoadGameAsync()
    {
        var saves = await saveService.GetAllSavesAsync();

        if (saves.Count == 0)
        {
            ui.ShowError("No saved games found.");
            AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
            Console.ReadKey(true);
            return MainMenuAction.ShowMenu();
        }

        var saveChoices = saves.Select(FormatSaveChoice).ToList();
        saveChoices.Add("[red]Cancel[/]");

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan1]Select a save to load:[/]")
                .AddChoices(saveChoices)
                .EnableSearch()
                .HighlightStyle(new Style(Color.Cyan1))
        );

        if (choice == "[red]Cancel[/]")
        {
            return MainMenuAction.ShowMenu();
        }

        var selectedSave = saves[saveChoices.IndexOf(choice)];
        return MainMenuAction.StartGame(selectedSave.Id);
    }

    private async Task<MainMenuAction> HandleDeleteSaveAsync()
    {
        var saves = await saveService.GetAllSavesAsync();

        if (saves.Count == 0)
        {
            ui.ShowError("No saved games found.");
            AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
            Console.ReadKey(true);
            return MainMenuAction.ShowMenu();
        }

        var saveChoices = saves.Select(FormatSaveChoice).ToList();
        saveChoices.Add("[red]Cancel[/]");

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[red]Select a save to DELETE:[/]")
                .AddChoices(saveChoices)
        );

        if (choice == "[red]Cancel[/]")
        {
            return MainMenuAction.ShowMenu();
        }

        var confirm = AnsiConsole.Confirm($"Are you sure you want to delete this save?");
        if (confirm)
        {
            var selectedSave = saves[saveChoices.IndexOf(choice)];
            await saveService.DeleteSaveAsync(selectedSave.Id);
            ui.ShowSuccess("Save deleted successfully.");
        }

        AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
        Console.ReadKey(true);
        return MainMenuAction.ShowMenu();
    }

    private static string FormatSaveChoice(GameSave save)
    {
        var status = save.IsCompleted
            ? (save.IsPlayerDead ? "[red](Dead)[/]" : "[green](Completed)[/]")
            : "[yellow](In Progress)[/]";

        var healthColor = save.Health > 60 ? "green" : save.Health > 30 ? "yellow" : "red";

        return $"{save.SlotName} {status} - {save.CurrentRoom.Name} - HP: [{healthColor}]{save.Health}[/] - {save.TurnCount} turns - Score: {save.Score} - {save.SavedAt:g}";
    }
}

/// <summary>
/// Represents the action to take after the main menu
/// </summary>
public class MainMenuAction
{
    public MenuActionType Type { get; private init; }
    public int SaveGameId { get; private init; }

    public static MainMenuAction StartGame(int saveId) => new() { Type = MenuActionType.StartGame, SaveGameId = saveId };
    public static MainMenuAction ShowMenu() => new() { Type = MenuActionType.ShowMenu };
    public static MainMenuAction Exit() => new() { Type = MenuActionType.Exit };
}

public enum MenuActionType
{
    StartGame,
    ShowMenu,
    Exit
}
