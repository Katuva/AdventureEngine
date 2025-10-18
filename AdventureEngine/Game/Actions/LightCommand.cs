using AdventureEngine.Models;
using AdventureEngine.Services;

namespace AdventureEngine.Game.Actions;

public class LightCommand : IGameCommand
{
    public string Name => "light";
    public string Description => "Light or ignite an item (e.g., lantern, torch)";
    public string[] Aliases => ["ignite", "kindle"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        if (input.DirectObjects.Count == 0)
        {
            return CommandResult.Error("Light what? Specify an item.");
        }

        var itemName = input.DirectObjects[0].ToLower();

        // Use SemanticResolver to find the item in inventory
        var resolver = new SemanticResolver(gameState.Context);
        var item = await resolver.ResolveItemAsync(
            itemName,
            gameState,
            includeInventory: true,
            includeRoom: false);

        if (item == null)
        {
            return CommandResult.Error($"You don't have '{itemName}' in your inventory.");
        }

        // Check current state
        var currentState = await gameState.GetItemStateAsync(item.Id);

        if (currentState == ItemStates.Lit)
        {
            return CommandResult.Ok($"The {item.Name} is already lit.");
        }

        // Light the item
        await gameState.SetItemStateAsync(item.Id, ItemStates.Lit);

        return CommandResult.Ok($"You light the {item.Name}. It casts a warm glow around you.");
    }
}
