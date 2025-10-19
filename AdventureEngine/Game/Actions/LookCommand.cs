using AdventureEngine.Models;
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

        // Check if room is dark and player has no light source
        if (room.IsDark && room.LightSourceItemId.HasValue)
        {
            var hasLightSource = await gameState.HasItemAsync(room.LightSourceItemId.Value);
            if (hasLightSource)
            {
                // Check if the light source is in the correct state (e.g., lit)
                var itemState = await gameState.GetItemStateAsync(room.LightSourceItemId.Value);
                if (itemState != ItemStates.Lit)
                {
                    return CommandResult.Error("It's too dark to see anything. You need a light source.");
                }
            }
            else
            {
                return CommandResult.Error("It's too dark to see anything. You need a light source.");
            }
        }

        // Use dynamic description resolver
        var descriptionResolver = new RoomDescriptionResolver(gameState.Context);
        var roomDescription = await descriptionResolver.GetRoomDescriptionAsync(room.Id, gameState);

        var description = roomDescription;

        // Get all items in the room using the helper method
        var allItems = await gameState.GetRoomItemsAsync(room.Id);

        // Get all visible examinable objects that should show in room description
        var visibleExaminables = await gameState.GetVisibleExaminableObjectsAsync(room.Id);
        var examinablesToShow = visibleExaminables.Where(eo => eo.ShowInRoomDescription).ToList();

        // Combine items and examinable objects for display
        var hasItemsOrObjects = allItems.Count > 0 || examinablesToShow.Count > 0;

        if (hasItemsOrObjects)
        {
            description += "\n\nYou can see:";

            // Add items
            foreach (var item in allItems)
            {
                var itemDesc = await gameState.GetItemDescriptionAsync(item);
                description += $"\n  - {item.Name}: {itemDesc}";
            }

            // Add examinable objects
            foreach (var examinable in examinablesToShow)
            {
                var displayName = examinable.DisplayName ?? examinable.Name;
                var lookDesc = await gameState.GetExaminableObjectDescriptionAsync(examinable, useLookDescription: true);
                description += $"\n  - {displayName}: {lookDesc}";
            }
        }

        // Exits are now shown via the compass display
        return CommandResult.Ok(description);
    }
}
