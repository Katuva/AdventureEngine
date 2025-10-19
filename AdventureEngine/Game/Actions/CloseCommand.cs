using AdventureEngine.Models;
using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class CloseCommand : IGameCommand
{
    public string Name => "close";
    public string Description => "Close a container";
    public string[] Aliases => ["shut"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        if (input.DirectObjects.Count == 0)
        {
            return CommandResult.Error("Close what? Specify a container (e.g., 'close chest').");
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

        // Get container state
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
        }

        // Check if already closed
        if (!state.IsOpen)
        {
            return CommandResult.Error($"The {container.Name} is already closed.");
        }

        // Close the container
        state.IsOpen = false;
        state.LastModified = DateTime.UtcNow;
        await gameState.Context.SaveChangesAsync();

        return CommandResult.Ok($"You close the {container.Name}.");
    }
}
