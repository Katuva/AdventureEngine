using AdventureEngine.Commands;
using Spectre.Console.Cli;

namespace AdventureEngine;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var app = new CommandApp<PlayCommand>();

        app.Configure(config =>
        {
            config.SetApplicationName("AdventureEngine");
            config.SetApplicationVersion("1.0.0");

            config.AddExample("AdventureEngine");
            config.AddExample("AdventureEngine", "--debug");
            config.AddExample("AdventureEngine", "-d");
        });

        return await app.RunAsync(args);
    }
}
