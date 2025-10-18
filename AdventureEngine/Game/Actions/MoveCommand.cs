using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class MoveCommand : IGameCommand
{
    private readonly string _direction;

    public MoveCommand(string direction)
    {
        _direction = direction.ToLower();
    }

    public string Name => _direction;
    public string Description => $"Move {_direction}";
    public string[] Aliases => _direction switch
    {
        "north" => ["n"],
        "south" => ["s"],
        "east" => ["e"],
        "west" => ["w"],
        "up" => ["u"],
        "down" => ["d"],
        _ => []
    };

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, string[] args)
    {
        var currentRoom = await gameState.GetCurrentRoomAsync();
        if (currentRoom == null)
        {
            return CommandResult.Error("You seem to be nowhere. This is a bug!");
        }

        int? nextRoomId = _direction switch
        {
            "north" => currentRoom.NorthRoomId,
            "south" => currentRoom.SouthRoomId,
            "east" => currentRoom.EastRoomId,
            "west" => currentRoom.WestRoomId,
            "up" => currentRoom.UpRoomId,
            "down" => currentRoom.DownRoomId,
            _ => null
        };

        if (!nextRoomId.HasValue)
        {
            return CommandResult.Error($"You can't go {_direction} from here.");
        }

        // Check if this room requires an examinable interaction to be unlocked
        var requiredInteraction = await gameState.Context.ExaminableObjects
            .FirstOrDefaultAsync(eo => eo.RoomId == currentRoom.Id &&
                                      eo.UnlocksRoomId == nextRoomId.Value);

        if (requiredInteraction != null)
        {
            // Check if the player has completed this interaction for this save
            var hasCompleted = await gameState.Context.CompletedExaminableInteractions
                .AnyAsync(cei => cei.GameSaveId == gameState.CurrentSaveId &&
                                cei.ExaminableObjectId == requiredInteraction.Id);

            if (!hasCompleted)
            {
                return CommandResult.Error($"You can't go {_direction} from here.");
            }
        }

        await gameState.MoveToRoomAsync(nextRoomId.Value);
        var newRoom = await gameState.GetCurrentRoomAsync();

        if (newRoom == null)
        {
            return CommandResult.Error("Something went wrong!");
        }

        var message = $"You move {_direction} to the {newRoom.Name}.\n\n{newRoom.Description}";

        // Check for deadly room damage
        if (newRoom.IsDeadlyRoom && newRoom.DamageAmount > 0)
        {
            var canSurvive = await gameState.CanSurviveDeadlyRoomAsync(newRoom.Id);

            DebugLogger.Log($"Room '{newRoom.Name}' is deadly. DamageAmount={newRoom.DamageAmount}, CanSurvive={canSurvive}");

            if (!canSurvive)
            {
                var healthBefore = await gameState.GetHealthAsync();
                await gameState.ModifyHealthAsync(-newRoom.DamageAmount);
                var healthAfter = await gameState.GetHealthAsync();

                DebugLogger.Log($"Applied {newRoom.DamageAmount} damage. Health before={healthBefore}, after={healthAfter}");

                message += $"\n\n⚠ {newRoom.DeathMessage ?? "This room is dangerous!"} You take {newRoom.DamageAmount} damage!";
                message += $"\nHealth: {healthAfter}";

                // Check if player died from this damage
                if (healthAfter <= 0)
                {
                    return CommandResult.Lose("Your health has reached zero. Game Over!");
                }
            }
            else
            {
                DebugLogger.Log($"Player can survive - has required item");
            }
        }
        else
        {
            DebugLogger.Log($"Room '{newRoom.Name}' - IsDeadly={newRoom.IsDeadlyRoom}, Damage={newRoom.DamageAmount}");
        }

        // Check for winning room
        if (newRoom.IsWinningRoom)
        {
            return CommandResult.Win(newRoom.WinMessage ?? "You won the game!");
        }

        return CommandResult.Ok(message);
    }
}
