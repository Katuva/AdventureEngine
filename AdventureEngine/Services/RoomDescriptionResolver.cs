using AdventureEngine.Data;
using AdventureEngine.Models;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Services;

/// <summary>
/// Resolves room descriptions based on game state conditions
/// </summary>
public class RoomDescriptionResolver(AdventureDbContext context)
{
    /// <summary>
    /// Get the appropriate room description based on current game state
    /// </summary>
    public async Task<string> GetRoomDescriptionAsync(int roomId, GameStateManager gameState)
    {
        // Get all descriptions for this room, ordered by priority (highest first)
        var descriptions = await context.RoomDescriptions
            .Include(rd => rd.RequiredItem)
            .Include(rd => rd.RequiredAction)
            .Where(rd => rd.RoomId == roomId)
            .OrderByDescending(rd => rd.Priority)
            .ToListAsync();

        // If no conditional descriptions exist, fall back to room.Description
        if (descriptions.Count == 0)
        {
            var room = await context.Rooms.FindAsync(roomId);
            return room?.Description ?? "You are in a room.";
        }

        // Check each description in priority order
        foreach (var desc in descriptions)
        {
            if (await EvaluateConditionAsync(desc, gameState))
            {
                return desc.Description;
            }
        }

        // Fallback: return the room's default description
        var fallbackRoom = await context.Rooms.FindAsync(roomId);
        return fallbackRoom?.Description ?? "You are in a room.";
    }

    /// <summary>
    /// Evaluate if a description's condition is met
    /// </summary>
    private async Task<bool> EvaluateConditionAsync(RoomDescription desc, GameStateManager gameState)
    {
        switch (desc.ConditionType.ToLower())
        {
            case DescriptionConditionTypes.Default:
                // Default always matches
                return true;

            case DescriptionConditionTypes.Always:
                // Always matches
                return true;

            case DescriptionConditionTypes.ItemState:
                // Requires item in specific state
                if (!desc.RequiredItemId.HasValue || string.IsNullOrEmpty(desc.RequiredItemState))
                {
                    return false;
                }

                var hasItem = await gameState.HasItemAsync(desc.RequiredItemId.Value);
                if (!hasItem && desc.ItemMustBeOwned)
                {
                    return false;
                }
                if (hasItem && !desc.ItemMustBeOwned)
                {
                    return false;
                }

                if (hasItem)
                {
                    var isInState = await gameState.IsItemInStateAsync(desc.RequiredItemId.Value, desc.RequiredItemState);
                    return isInState;
                }

                return false;

            case DescriptionConditionTypes.HasItem:
                // Requires player to have (or not have) an item
                if (!desc.RequiredItemId.HasValue)
                {
                    return false;
                }

                var playerHasItem = await gameState.HasItemAsync(desc.RequiredItemId.Value);
                return desc.ItemMustBeOwned ? playerHasItem : !playerHasItem;

            case DescriptionConditionTypes.CompletedAction:
                // Requires action to be completed (or not completed)
                if (!desc.RequiredActionId.HasValue)
                {
                    return false;
                }

                var isCompleted = await context.CompletedActions
                    .AnyAsync(ca => ca.GameSaveId == gameState.CurrentSaveId &&
                                   ca.RoomActionId == desc.RequiredActionId.Value);

                return desc.ActionMustBeCompleted ? isCompleted : !isCompleted;

            default:
                return false;
        }
    }
}
