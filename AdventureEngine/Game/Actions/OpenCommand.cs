using AdventureEngine.Models;
using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class OpenCommand : IGameCommand
{
    public string Name => "open";
    public string Description => "Open a container";
    public string[] Aliases => [];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        if (input.DirectObjects.Count == 0)
        {
            return CommandResult.Error("Open what? Specify a container (e.g., 'open chest').");
        }

        var containerName = input.DirectObjects[0].ToLower();
        var room = await gameState.GetCurrentRoomAsync();

        if (room == null)
        {
            return CommandResult.Error("You seem to be nowhere. This is a bug!");
        }

        // Find the container
        var containers = await gameState.Context.Containers
            .Where(c => c.RoomId == room.Id)
            .ToListAsync();

        var container = containers.FirstOrDefault(c =>
            c.Name.ToLower() == containerName ||
            (c.Keywords != null && c.Keywords.ToLower().Split(',').Any(k => k.Trim() == containerName)));

        if (container == null)
        {
            return CommandResult.Error($"There is no '{containerName}' here.");
        }

        // Get or create container state
        var state = await gameState.Context.ContainerStates
            .FirstOrDefaultAsync(cs => cs.GameSaveId == gameState.CurrentSaveId && cs.ContainerId == container.Id);

        if (state == null)
        {
            // Initialize state from container defaults
            state = new ContainerState
            {
                GameSaveId = gameState.CurrentSaveId,
                ContainerId = container.Id,
                IsOpen = container.StartsOpen,
                IsLocked = container.StartsLocked
            };
            gameState.Context.ContainerStates.Add(state);
            await gameState.Context.SaveChangesAsync();
        }

        // Check if already open
        if (state.IsOpen)
        {
            return CommandResult.Error($"The {container.Name} is already open.");
        }

        // Check if locked
        if (state.IsLocked)
        {
            var message = container.LockedMessage ?? $"The {container.Name} is locked.";
            return CommandResult.Error(message);
        }

        // Open the container
        state.IsOpen = true;
        state.LastModified = DateTime.UtcNow;
        await gameState.Context.SaveChangesAsync();

        // Get items in container
        var containerItems = await gameState.Context.ContainerItems
            .Include(ci => ci.Item)
            .Where(ci => ci.ContainerId == container.Id)
            .ToListAsync();

        var response = $"You open the {container.Name}.";

        if (containerItems.Count > 0)
        {
            var itemList = string.Join(", ", containerItems.Select(ci => ci.Item.Name));
            response += $"\n\nInside you see: {itemList}";
        }
        else
        {
            var emptyMsg = container.EmptyDescription ?? "It's empty.";
            response += $"\n\n{emptyMsg}";
        }

        return CommandResult.Ok(response);
    }
}
