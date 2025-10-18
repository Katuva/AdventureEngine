using Spectre.Console;
using AdventureEngine.Config;
using AdventureEngine.Services;

namespace AdventureEngine.UI;

/// <summary>
/// Handles all console UI rendering using Spectre.Console
/// </summary>
public class ConsoleUI(GameConfiguration config)
{
    public void ShowGameTitle()
    {
        var rule = new Rule($"[{config.UI.TitleColor}]{config.GameName}[/]")
        {
            Justification = Justify.Center
        };
        
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();
    }

    public void ShowIntro()
    {
        AnsiConsole.Clear();

        ShowGameTitle();
        
        AnsiConsole.MarkupLine($"[{config.UI.DescriptionColor} italic]{config.GameDescription}[/]");
        AnsiConsole.MarkupLine($"[dim]By {config.Author} | Version {config.Version}[/]");
        AnsiConsole.WriteLine();
    }

    public void ShowRoomHeader(string roomName)
    {
        var panel = new Panel($"[bold {config.UI.TitleColor}]{roomName}[/]")
        {
            Border = BoxBorder.Double,
            BorderStyle = new Style(Color.Cyan1)
        };
        AnsiConsole.Write(panel);
    }

    public void ShowMessage(string message, MessageType type = MessageType.Normal)
    {
        var color = type switch
        {
            MessageType.Success => config.UI.SuccessColor,
            MessageType.Error => config.UI.ErrorColor,
            MessageType.Warning => config.UI.WarningColor,
            MessageType.Normal => config.UI.DescriptionColor,
            _ => "white"
        };

        AnsiConsole.MarkupLine($"[{color}]{Markup.Escape(message)}[/]");
    }

    public void ShowError(string message)
    {
        ShowMessage($"ERROR: {message}", MessageType.Error);
    }

    public void ShowSuccess(string message)
    {
        ShowMessage(message, MessageType.Success);
    }

    public void ShowWarning(string message)
    {
        ShowMessage(message, MessageType.Warning);
    }

    public void ShowHealthBar(int currentHealth, int maxHealth)
    {
        var percentage = (double)currentHealth / maxHealth;
        var barWidth = 30;
        var filledWidth = (int)(barWidth * percentage);

        var color = percentage switch
        {
            > 0.6 => "green",
            > 0.3 => "yellow",
            _ => "red"
        };

        var bar = new string('█', filledWidth) + new string('░', barWidth - filledWidth);
        AnsiConsole.MarkupLine($"[bold]Health:[/] [{color}]{bar}[/] {currentHealth}/{maxHealth}");
    }

    public string GetInput(string prompt = ">")
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>($"[{config.UI.PromptColor}]{prompt}[/]")
                .AllowEmpty()
        );
    }

    public void ShowGameOver(bool won, string message)
    {
        AnsiConsole.Clear();

        if (won)
        {
            var panel = new Panel(
                Align.Center(
                    new Markup($"[bold {config.UI.SuccessColor}]VICTORY![/]\n\n{Markup.Escape(message)}")
                )
            )
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Green)
            };
            AnsiConsole.Write(panel);
        }
        else
        {
            var panel = new Panel(
                Align.Center(
                    new Markup($"[bold {config.UI.ErrorColor}]GAME OVER[/]\n\n{Markup.Escape(message)}")
                )
            )
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Red)
            };
            AnsiConsole.Write(panel);
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim]Press any key to return to the main menu...[/]");
        Console.ReadKey(true);
    }

    public void ShowTable<T>(string title, IEnumerable<T> data, params (string Header, Func<T, string> Value)[] columns)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[{config.UI.TitleColor}]{title}[/]");

        foreach (var (header, _) in columns)
        {
            table.AddColumn(header);
        }

        foreach (var item in data)
        {
            var values = columns.Select(c => c.Value(item)).ToArray();
            table.AddRow(values);
        }

        AnsiConsole.Write(table);
    }

    public void DrawSeparator()
    {
        AnsiConsole.Write(new Rule() { Style = new Style(Color.Grey) });
    }

    public async Task ShowCompassAsync(AdventureEngine.Models.Room room, GameStateManager gameState)
    {
        // Get console width for centering
        var consoleWidth = Console.WindowWidth;

        // Check which rooms have been visited
        var upVisited = room.UpRoomId.HasValue && await gameState.HasVisitedRoomAsync(room.UpRoomId.Value);
        var downVisited = room.DownRoomId.HasValue && await gameState.HasVisitedRoomAsync(room.DownRoomId.Value);
        var northVisited = room.NorthRoomId.HasValue && await gameState.HasVisitedRoomAsync(room.NorthRoomId.Value);
        var southVisited = room.SouthRoomId.HasValue && await gameState.HasVisitedRoomAsync(room.SouthRoomId.Value);
        var eastVisited = room.EastRoomId.HasValue && await gameState.HasVisitedRoomAsync(room.EastRoomId.Value);
        var westVisited = room.WestRoomId.HasValue && await gameState.HasVisitedRoomAsync(room.WestRoomId.Value);

        // Build compass components with color coding
        // Unvisited (new areas): bright cyan/white
        // Visited: dim grey
        var upChar = room.UpRoomId.HasValue ? (upVisited ? "[grey]U[/]" : "[bold cyan]U[/]") : " ";
        var downChar = room.DownRoomId.HasValue ? (downVisited ? "[grey]D[/]" : "[bold cyan]D[/]") : " ";
        var northChar = room.NorthRoomId.HasValue ? (northVisited ? "[grey]N[/]" : "[bold cyan]N[/]") : " ";
        var southChar = room.SouthRoomId.HasValue ? (southVisited ? "[grey]S[/]" : "[bold cyan]S[/]") : " ";
        var eastChar = room.EastRoomId.HasValue ? (eastVisited ? "[grey]E[/]" : "[bold cyan]E[/]") : " ";
        var westChar = room.WestRoomId.HasValue ? (westVisited ? "[grey]W[/]" : "[bold cyan]W[/]") : " ";

        var center = room.UpRoomId.HasValue || room.DownRoomId.HasValue ? "┼" : "+";

        // Build the compass parts
        // Layout:  N (aligned with +)     U (to the right)
        //          W + E (with full-width rule)
        //          S (aligned with +)     D (to the right)

        var middlePart = $" {westChar} [dim]{center}[/] {eastChar} ";  // " W + E "
        var middleWidth = 7; // Account for visible characters only
        var middlePadding = Math.Max(0, (consoleWidth - middleWidth) / 2);

        var centerPosition = middlePadding + 3;

        // Top part: N aligned with +, then spaces, then U
        var topPart = $"{northChar}     {upChar}";
        var topLeftPadding = centerPosition;

        // Bottom part: S aligned with +, then spaces, then D
        var bottomPart = $"{southChar}     {downChar}";
        var bottomLeftPadding = centerPosition;

        // Display top line (N aligned with +, U to the right)
        var topSpacing = new string(' ', topLeftPadding);
        AnsiConsole.MarkupLine($"{topSpacing}{topPart}");

        // Display middle line with full-width rule
        var leftRuleWidth = middlePadding;
        var rightRuleWidth = consoleWidth - middlePadding - middleWidth;
        var leftRule = new string('─', Math.Max(0, leftRuleWidth));
        var rightRule = new string('─', Math.Max(0, rightRuleWidth));

        AnsiConsole.MarkupLine($"[dim]{leftRule}[/]{middlePart}[dim]{rightRule}[/]");

        // Display bottom line (S aligned with +, D to the right)
        var bottomSpacing = new string(' ', bottomLeftPadding);
        AnsiConsole.MarkupLine($"{bottomSpacing}{bottomPart}");
    }
}

public enum MessageType
{
    Normal,
    Success,
    Error,
    Warning
}
