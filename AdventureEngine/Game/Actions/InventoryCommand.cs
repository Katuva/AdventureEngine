using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class InventoryCommand : IGameCommand
{
    public string Name => "inventory";
    public string Description => "Show your inventory";
    public string[] Aliases => ["i", "inv"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        var inventory = await gameState.Context.InventoryItems
            .Include(ii => ii.Item)
            .Where(ii => ii.GameSaveId == gameState.CurrentSaveId)
            .ToListAsync();

        if (inventory.Count == 0)
        {
            return CommandResult.Ok("Your inventory is empty.");
        }

        var message = "You are carrying:\n";
        foreach (var invItem in inventory)
        {
            var itemDesc = await gameState.GetItemDescriptionAsync(invItem.Item);
            message += $"  - {invItem.Item.Name}: {itemDesc}\n";
        }

        return CommandResult.Ok(message.TrimEnd());
    }
}
