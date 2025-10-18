using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AdventureEngine.Config;

/// <summary>
/// Loads game configuration from YAML file
/// </summary>
public class ConfigurationLoader
{
    private const string DefaultConfigFileName = "game-config.yaml";

    public static GameConfiguration LoadConfiguration(string? configPath = null)
    {
        configPath ??= DefaultConfigFileName;

        if (!File.Exists(configPath))
        {
            CreateDefaultConfiguration(configPath);
        }

        var yaml = File.ReadAllText(configPath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<GameConfiguration>(yaml);
    }

    private static void CreateDefaultConfiguration(string path)
    {
        var defaultConfig = new GameConfiguration
        {
            GameName = "Mystery Mansion Adventure",
            GameDescription = "Explore a mysterious mansion and uncover its secrets",
            Author = "Adventure Engine",
            Version = "1.0.0",
            MaxInventoryWeight = 100,
            MaxSaveSlots = 5,
            DatabasePath = "adventure.db"
        };

        var serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        var yaml = serializer.Serialize(defaultConfig);
        File.WriteAllText(path, yaml);
    }
}
