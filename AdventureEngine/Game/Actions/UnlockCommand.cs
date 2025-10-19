using AdventureEngine.Models;
using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class UnlockCommand : IGameCommand
{
    public string Name => "unlock";
    public string Description => "Unlock a container with a key";
    public string[] Aliases => [];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        if (input.DirectObjects.Count == 0)
        {
            return CommandResult.Error("Unlock what? Specify a container (e.g., 'unlock chest').");
        }

        var containerName = input.DirectObjects[0].ToLower();
        var room = await gameState.GetCurrentRoomAsync();

        if (room == null)
        {
            return CommandResult.Error("You seem to be nowhere. This is a bug!");
        }

        // Find the container
        var containers = await gameState.Context.Containers
            .Include(c => c.KeyItem)
            .Where(c => c.RoomId == room.Id)
            .ToListAsync();

        var container = containers.FirstOrDefault(c =>
            c.Name.ToLower() == containerName ||
            (c.Keywords != null && c.Keywords.ToLower().Split(',').Any(k => k.Trim() == containerName)));

        if (container == null)
        {
            return CommandResult.Error($"There is no '{containerName}' here.");
        }

        if (!container.IsLockable)
        {
            return CommandResult.Error($"The {container.Name} cannot be locked or unlocked.");
        }

        // Get container state
        var state = await gameState.Context.ContainerStates
            .FirstOrDefaultAsync(cs => cs.GameSaveId == gameState.CurrentSaveId && cs.ContainerId == container.Id);

        if (state == null)
        {
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

        // Check if already unlocked
        if (!state.IsLocked)
        {
            return CommandResult.Error($"The {container.Name} is already unlocked.");
        }

        // Check if requires a key
        if (container.KeyItemId.HasValue)
        {
            var hasKey = await gameState.HasItemAsync(container.KeyItemId.Value);
            if (!hasKey)
            {
                return CommandResult.Error($"You don't have the right key to unlock the {container.Name}.");
            }
        }

        // Unlock the container
        state.IsLocked = false;
        state.LastModified = DateTime.UtcNow;
        await gameState.Context.SaveChangesAsync();

        var message = container.UnlockMessage ?? $"You unlock the {container.Name}.";
        return CommandResult.Ok(message);
    }
}
