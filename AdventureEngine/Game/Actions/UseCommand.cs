using AdventureEngine.Models;
using AdventureEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace AdventureEngine.Game.Actions;

public class UseCommand : IGameCommand
{
    public string Name => "use";
    public string Description => "Use an item from your inventory (optionally 'on' something)";
    public string[] Aliases => ["activate"];

    public async Task<CommandResult> ExecuteAsync(GameStateManager gameState, ParsedInput input)
    {
        if (input.DirectObjects.Count == 0)
        {
            return CommandResult.Error("Use what? Specify an item name.");
        }

        // Get item name and target from parsed input
        var itemName = input.DirectObjects[0].ToLower();
        var targetName = input.IndirectObject?.ToLower();

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

        var room = await gameState.GetCurrentRoomAsync();
        if (room == null)
        {
            return CommandResult.Error("You seem to be nowhere. This is a bug!");
        }

        // If using on a target, check for interactions
        if (targetName != null)
        {
            // First, check for examinable object interactions using resolver
            var examinableObject = await resolver.ResolveExaminableObjectAsync(targetName, room.Id, gameState);

            // Check if this examinable requires our item
            if (examinableObject != null && examinableObject.RequiredItemId != item.Id)
            {
                examinableObject = null; // Wrong item for this object
            }

            // Load the unlocks room if needed
            if (examinableObject is { UnlocksRoomId: not null })
            {
                await gameState.Context.Entry(examinableObject)
                    .Reference(eo => eo.UnlocksRoom)
                    .LoadAsync();
            }

            if (examinableObject != null)
            {
                // Check if already completed for this save
                var alreadyCompleted = await gameState.Context.CompletedExaminableInteractions
                    .AnyAsync(cei => cei.GameSaveId == gameState.CurrentSaveId &&
                                    cei.ExaminableObjectId == examinableObject.Id);

                if (alreadyCompleted)
                {
                    return CommandResult.Error("You've already done that.");
                }

                // Unlock room if applicable (for ExaminableObjects)
                if (examinableObject.UnlocksRoomId.HasValue)
                {
                    var currentRoom = await gameState.Context.Rooms.FindAsync(room.Id);
                    if (currentRoom != null)
                    {
                        // Update the room connection based on the unlock direction
                        var direction = examinableObject.UnlockDirection?.ToLower() ?? "up";
                        switch (direction)
                        {
                            case "north":
                                currentRoom.NorthRoomId = examinableObject.UnlocksRoomId.Value;
                                break;
                            case "south":
                                currentRoom.SouthRoomId = examinableObject.UnlocksRoomId.Value;
                                break;
                            case "east":
                                currentRoom.EastRoomId = examinableObject.UnlocksRoomId.Value;
                                break;
                            case "west":
                                currentRoom.WestRoomId = examinableObject.UnlocksRoomId.Value;
                                break;
                            case "up":
                                currentRoom.UpRoomId = examinableObject.UnlocksRoomId.Value;
                                break;
                            case "down":
                                currentRoom.DownRoomId = examinableObject.UnlocksRoomId.Value;
                                break;
                        }
                        await gameState.Context.SaveChangesAsync();
                    }
                }

                // Mark as completed for this save
                var completedInteraction = new CompletedExaminableInteraction
                {
                    GameSaveId = gameState.CurrentSaveId,
                    ExaminableObjectId = examinableObject.Id,
                    CompletedAt = DateTime.UtcNow
                };
                gameState.Context.CompletedExaminableInteractions.Add(completedInteraction);
                await gameState.Context.SaveChangesAsync();

                return CommandResult.Ok(examinableObject.SuccessMessage ?? $"You use the {item.Name} on the {examinableObject.Name}.");
            }

            return CommandResult.Error($"You can't use the {item.Name} on {targetName}.");
        }

        // Check if this is a healing item
        if (item.HealingAmount > 0)
        {
            // Get usage count for this item in this save
            var itemUsage = await gameState.Context.ItemUsages
                .FirstOrDefaultAsync(iu => iu.GameSaveId == gameState.CurrentSaveId && iu.ItemId == item.Id);

            var usesLeft = item.MaxUses - (itemUsage?.UsesCount ?? 0);

            // Check if item is exhausted (and not unlimited)
            if (item.MaxUses > 0 && usesLeft <= 0)
            {
                var emptyDesc = item.EmptyDescription ?? $"The {item.Name} is empty and cannot be used anymore.";
                return CommandResult.Error(emptyDesc);
            }

            // Check if player is at max health
            var currentHealth = await gameState.GetHealthAsync();
            var maxHealth = gameState.Config.MaxHealth;

            if (currentHealth >= maxHealth)
            {
                return CommandResult.Error("You're already at full health.");
            }

            // Apply healing
            await gameState.ModifyHealthAsync(item.HealingAmount);
            var newHealth = await gameState.GetHealthAsync();

            // Track usage
            if (item.MaxUses > 0) // Only track if not unlimited
            {
                if (itemUsage == null)
                {
                    itemUsage = new ItemUsage
                    {
                        GameSaveId = gameState.CurrentSaveId,
                        ItemId = item.Id,
                        UsesCount = 1,
                        LastUsedAt = DateTime.UtcNow
                    };
                    gameState.Context.ItemUsages.Add(itemUsage);
                }
                else
                {
                    itemUsage.UsesCount++;
                    itemUsage.LastUsedAt = DateTime.UtcNow;
                }
                await gameState.Context.SaveChangesAsync();

                // Check if item is now empty
                if (itemUsage.UsesCount >= item.MaxUses)
                {
                    // Set item state to empty
                    await gameState.SetItemStateAsync(item.Id, ItemStates.Empty);
                }
            }

            var healMessage = item.UseMessage ?? $"You use the {item.Name} and restore {item.HealingAmount} health.";
            healMessage += $"\nHealth: {newHealth}";

            // Check if item should be removed from inventory
            if (item.MaxUses > 0)
            {
                var remaining = item.MaxUses - (itemUsage?.UsesCount ?? 0);
                if (remaining > 0)
                {
                    healMessage += $"\n({remaining} use{(remaining == 1 ? "" : "s")} remaining)";
                }
                else if (item.DisappearsWhenEmpty)
                {
                    // Item is empty - remove from inventory if configured to disappear
                    var inventoryItem = await gameState.Context.InventoryItems
                        .FirstOrDefaultAsync(ii => ii.GameSaveId == gameState.CurrentSaveId && ii.ItemId == item.Id);

                    if (inventoryItem != null)
                    {
                        gameState.Context.InventoryItems.Remove(inventoryItem);

                        // Track as permanently removed so it doesn't reappear in original room
                        var removedItem = new RemovedItem
                        {
                            GameSaveId = gameState.CurrentSaveId,
                            ItemId = item.Id,
                            RemovedAt = DateTime.UtcNow
                        };
                        gameState.Context.RemovedItems.Add(removedItem);

                        await gameState.Context.SaveChangesAsync();
                    }
                }
                else
                {
                    // Item is empty but doesn't disappear
                    healMessage += $"\n(The {item.Name} is now empty)";
                }
            }

            return CommandResult.Ok(healMessage);
        }

        // Default use behavior
        var message = item.UseMessage ?? $"You use the {item.Name}.";
        return CommandResult.Ok(message);
    }
}
