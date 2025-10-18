using AdventureEngine.Services;

namespace AdventureEngine.Game.Actions;

public class LookCommand : IGameCommand
{
    public string Name => "look";
    public string Description => "Look around the current room";
    public string[] Aliases => ["l", "examine"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        var room = await gameState.GetCurrentRoomAsync();
        if (room == null)
        {
            return CommandResult.Error("You seem to be nowhere. This is a bug!");
        }

        // Use dynamic description resolver
        var descriptionResolver = new RoomDescriptionResolver(gameState.Context);
        var roomDescription = await descriptionResolver.GetRoomDescriptionAsync(room.Id, gameState);

        var description = roomDescription;

        // Get all items in the room using the helper method
        var allItems = await gameState.GetRoomItemsAsync(room.Id);

        if (allItems.Any())
        {
            description += "\n\nYou can see:";
            foreach (var item in allItems)
            {
                description += $"\n  - {item.Name}: {item.Description}";
            }
        }

        // Exits are now shown via the compass display
        return CommandResult.Ok(description);
    }
}
