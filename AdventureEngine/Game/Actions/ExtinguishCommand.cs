using AdventureEngine.Models;
using AdventureEngine.Services;

namespace AdventureEngine.Game.Actions;

public class ExtinguishCommand : IGameCommand
{
    public string Name => "extinguish";
    public string Description => "Extinguish or put out a light source";
    public string[] Aliases => ["unlight", "snuff", "blow out"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        if (input.DirectObjects.Count == 0)
        {
            return CommandResult.Error("Extinguish what? Specify an item.");
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

        if (currentState != ItemStates.Lit)
        {
            return CommandResult.Ok($"The {item.Name} isn't lit.");
        }

        // Extinguish the item
        await gameState.SetItemStateAsync(item.Id, ItemStates.Unlit);

        return CommandResult.Ok($"You extinguish the {item.Name}. Darkness surrounds you.");
    }
}
