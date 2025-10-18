using Spectre.Console;
using AdventureEngine.Config;
using AdventureEngine.Services;

namespace AdventureEngine.UI;

/// <summary>
/// Handles all console UI rendering using Spectre.Console
/// </summary>
public class ConsoleUI
{
    private readonly GameConfiguration _config;

    public ConsoleUI(GameConfiguration config)
    {
        _config = config;
    }

    public void ShowTitle()
    {
        AnsiConsole.Clear();

        var rule = new Rule($"[{_config.UI.TitleColor}]{_config.GameName}[/]")
        {
            Justification = Justify.Center
        };
        AnsiConsole.Write(rule);

        AnsiConsole.MarkupLine($"[{_config.UI.DescriptionColor} italic]{_config.GameDescription}[/]");
        AnsiConsole.MarkupLine($"[dim]By {_config.Author} | Version {_config.Version}[/]");
        AnsiConsole.WriteLine();
    }

    public void ShowRoomHeader(string roomName)
    {
        var panel = new Panel($"[bold {_config.UI.TitleColor}]{roomName}[/]")
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
            MessageType.Success => _config.UI.SuccessColor,
            MessageType.Error => _config.UI.ErrorColor,
            MessageType.Warning => _config.UI.WarningColor,
            MessageType.Normal => _config.UI.DescriptionColor,
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
            new TextPrompt<string>($"[{_config.UI.PromptColor}]{prompt}[/]")
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
                    new Markup($"[bold {_config.UI.SuccessColor}]VICTORY![/]\n\n{Markup.Escape(message)}")
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
                    new Markup($"[bold {_config.UI.ErrorColor}]GAME OVER[/]\n\n{Markup.Escape(message)}")
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
            .Title($"[{_config.UI.TitleColor}]{title}[/]");

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

    public void ShowCompass(AdventureEngine.Models.Room room)
    {
        // Get console width for centering
        var consoleWidth = Console.WindowWidth;

        // Build compass components
        var upChar = room.UpRoomId.HasValue ? "U" : " ";
        var downChar = room.DownRoomId.HasValue ? "D" : " ";
        var northChar = room.NorthRoomId.HasValue ? "N" : " ";
        var southChar = room.SouthRoomId.HasValue ? "S" : " ";
        var eastChar = room.EastRoomId.HasValue ? "E" : " ";
        var westChar = room.WestRoomId.HasValue ? "W" : " ";

        var center = room.UpRoomId.HasValue || room.DownRoomId.HasValue ? "┼" : "+";
        var udBar = room.UpRoomId.HasValue || room.DownRoomId.HasValue ? "│" : "-";

        // Build the compass parts
        // Layout:  N (aligned with +)     U (to the right)
        //          W + E (with full-width rule)
        //          S (aligned with +)     D (to the right)

        var middlePart = $" {westChar} {center} {eastChar} ";  // " W + E "
        var middleWidth = middlePart.Length;
        var middlePadding = Math.Max(0, (consoleWidth - middleWidth) / 2);

        // N and S should align with the center (+), which is at position middlePadding + 3
        // (3 = space + W + space before the +)
        var centerPosition = middlePadding + 3;

        // Top part: N aligned with +, then spaces, then U
        var topPart = $"{northChar}     {upChar}";
        var topLeftPadding = centerPosition; // N aligns with the +

        // Bottom part: S aligned with +, then spaces, then D
        var bottomPart = $"{southChar}     {downChar}";
        var bottomLeftPadding = centerPosition; // S aligns with the +

        // Display top line (N aligned with +, U to the right)
        var topSpacing = new string(' ', topLeftPadding);
        AnsiConsole.MarkupLine($"[dim]{topSpacing}{Markup.Escape(topPart)}[/]");

        // Display middle line with full-width rule
        var leftRuleWidth = middlePadding;
        var rightRuleWidth = consoleWidth - middlePadding - middleWidth;
        var leftRule = new string('─', Math.Max(0, leftRuleWidth));
        var rightRule = new string('─', Math.Max(0, rightRuleWidth));

        AnsiConsole.MarkupLine($"[dim]{leftRule}{Markup.Escape(middlePart)}{rightRule}[/]");

        // Display bottom line (S aligned with +, D to the right)
        var bottomSpacing = new string(' ', bottomLeftPadding);
        DebugLogger.Log($"About to display bottom line: '{bottomPart}'");
        AnsiConsole.MarkupLine($"[dim]{bottomSpacing}{Markup.Escape(bottomPart)}[/]");
        DebugLogger.Log("Bottom line displayed");
    }
}

public enum MessageType
{
    Normal,
    Success,
    Error,
    Warning
}
