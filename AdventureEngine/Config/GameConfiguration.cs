namespace AdventureEngine.Config;

/// <summary>
/// Game configuration loaded from YAML
/// </summary>
public class GameConfiguration
{
    public required string GameName { get; set; }
    public required string GameDescription { get; set; }
    public required string Author { get; set; }
    public required string Version { get; set; }
    public int MaxInventoryWeight { get; set; } = 100;
    public int MaxSaveSlots { get; set; } = 5;
    public int StartingHealth { get; set; } = 100;
    public string DatabasePath { get; set; } = "adventure.db";
    public UISettings UI { get; set; } = new();
}

public class UISettings
{
    public string TitleColor { get; set; } = "cyan";
    public string DescriptionColor { get; set; } = "white";
    public string ErrorColor { get; set; } = "red";
    public string SuccessColor { get; set; } = "green";
    public string WarningColor { get; set; } = "yellow";
    public string PromptColor { get; set; } = "blue";
}
