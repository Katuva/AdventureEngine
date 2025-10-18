using Microsoft.EntityFrameworkCore;
using AdventureEngine.Models;

namespace AdventureEngine.Data;

/// <summary>
/// Entity Framework database context for the adventure game
/// </summary>
public class AdventureDbContext(DbContextOptions<AdventureDbContext> options) : DbContext(options)
{
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<RoomAction> RoomActions => Set<RoomAction>();
    public DbSet<GameSave> GameSaves => Set<GameSave>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<CompletedAction> CompletedActions => Set<CompletedAction>();
    public DbSet<PlacedItem> PlacedItems => Set<PlacedItem>();
    public DbSet<ExaminableObject> ExaminableObjects => Set<ExaminableObject>();
    public DbSet<CompletedExaminableInteraction> CompletedExaminableInteractions => Set<CompletedExaminableInteraction>();
    public DbSet<Vocabulary> Vocabularies => Set<Vocabulary>();
    public DbSet<ItemAdjective> ItemAdjectives => Set<ItemAdjective>();
    public DbSet<PlayerContext> PlayerContexts => Set<PlayerContext>();
    public DbSet<ItemState> ItemStates => Set<ItemState>();
    public DbSet<RoomDescription> RoomDescriptions => Set<RoomDescription>();
    public DbSet<VisitedRoom> VisitedRooms => Set<VisitedRoom>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Room navigation relationships to prevent cycles
        modelBuilder.Entity<Room>()
            .HasOne(r => r.NorthRoom)
            .WithMany()
            .HasForeignKey(r => r.NorthRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.SouthRoom)
            .WithMany()
            .HasForeignKey(r => r.SouthRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.EastRoom)
            .WithMany()
            .HasForeignKey(r => r.EastRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.WestRoom)
            .WithMany()
            .HasForeignKey(r => r.WestRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.UpRoom)
            .WithMany()
            .HasForeignKey(r => r.UpRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.DownRoom)
            .WithMany()
            .HasForeignKey(r => r.DownRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Item-Room relationship
        modelBuilder.Entity<Item>()
            .HasOne(i => i.Room)
            .WithMany(r => r.Items)
            .HasForeignKey(i => i.RoomId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure RoomAction relationships
        modelBuilder.Entity<RoomAction>()
            .HasOne(ra => ra.Room)
            .WithMany(r => r.Actions)
            .HasForeignKey(ra => ra.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RoomAction>()
            .HasOne(ra => ra.RequiredItem)
            .WithMany()
            .HasForeignKey(ra => ra.RequiredItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoomAction>()
            .HasOne(ra => ra.UnlocksRoom)
            .WithMany()
            .HasForeignKey(ra => ra.UnlocksRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure GameSave
        modelBuilder.Entity<GameSave>()
            .HasOne(gs => gs.CurrentRoom)
            .WithMany()
            .HasForeignKey(gs => gs.CurrentRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GameSave>()
            .HasIndex(gs => gs.SlotName)
            .IsUnique();

        // Configure InventoryItem
        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.GameSave)
            .WithMany(gs => gs.Inventory)
            .HasForeignKey(ii => ii.GameSaveId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Item)
            .WithMany(i => i.InventoryItems)
            .HasForeignKey(ii => ii.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure CompletedAction
        modelBuilder.Entity<CompletedAction>()
            .HasOne(ca => ca.GameSave)
            .WithMany(gs => gs.CompletedActions)
            .HasForeignKey(ca => ca.GameSaveId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CompletedAction>()
            .HasOne(ca => ca.RoomAction)
            .WithMany()
            .HasForeignKey(ca => ca.RoomActionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure PlacedItem
        modelBuilder.Entity<PlacedItem>()
            .HasOne(pi => pi.GameSave)
            .WithMany()
            .HasForeignKey(pi => pi.GameSaveId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlacedItem>()
            .HasOne(pi => pi.Item)
            .WithMany()
            .HasForeignKey(pi => pi.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PlacedItem>()
            .HasOne(pi => pi.Room)
            .WithMany()
            .HasForeignKey(pi => pi.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure ExaminableObject
        modelBuilder.Entity<ExaminableObject>()
            .HasOne(eo => eo.Room)
            .WithMany()
            .HasForeignKey(eo => eo.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExaminableObject>()
            .HasOne(eo => eo.RevealedByAction)
            .WithMany()
            .HasForeignKey(eo => eo.RevealedByActionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExaminableObject>()
            .HasOne(eo => eo.RequiredItem)
            .WithMany()
            .HasForeignKey(eo => eo.RequiredItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExaminableObject>()
            .HasOne(eo => eo.UnlocksRoom)
            .WithMany()
            .HasForeignKey(eo => eo.UnlocksRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure CompletedExaminableInteraction
        modelBuilder.Entity<CompletedExaminableInteraction>()
            .HasOne(cei => cei.GameSave)
            .WithMany()
            .HasForeignKey(cei => cei.GameSaveId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CompletedExaminableInteraction>()
            .HasOne(cei => cei.ExaminableObject)
            .WithMany()
            .HasForeignKey(cei => cei.ExaminableObjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Vocabulary
        modelBuilder.Entity<Vocabulary>()
            .HasIndex(v => new { v.Word, v.WordType })
            .IsUnique();

        // Configure ItemAdjective
        modelBuilder.Entity<ItemAdjective>()
            .HasOne(ia => ia.Item)
            .WithMany()
            .HasForeignKey(ia => ia.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ItemAdjective>()
            .HasIndex(ia => new { ia.ItemId, ia.Adjective })
            .IsUnique();

        // Configure PlayerContext
        modelBuilder.Entity<PlayerContext>()
            .HasOne(pc => pc.GameSave)
            .WithMany()
            .HasForeignKey(pc => pc.GameSaveId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlayerContext>()
            .HasOne(pc => pc.LastMentionedItem)
            .WithMany()
            .HasForeignKey(pc => pc.LastMentionedItemId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PlayerContext>()
            .HasOne(pc => pc.LastExaminedObject)
            .WithMany()
            .HasForeignKey(pc => pc.LastExaminedObjectId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PlayerContext>()
            .HasOne(pc => pc.LastRoom)
            .WithMany()
            .HasForeignKey(pc => pc.LastRoomId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PlayerContext>()
            .HasIndex(pc => pc.GameSaveId)
            .IsUnique();

        // Configure ItemState
        modelBuilder.Entity<ItemState>()
            .HasOne(ist => ist.GameSave)
            .WithMany()
            .HasForeignKey(ist => ist.GameSaveId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ItemState>()
            .HasOne(ist => ist.Item)
            .WithMany()
            .HasForeignKey(ist => ist.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ItemState>()
            .HasIndex(ist => new { ist.GameSaveId, ist.ItemId })
            .IsUnique();

        // Configure RoomDescription
        modelBuilder.Entity<RoomDescription>()
            .HasOne(rd => rd.Room)
            .WithMany()
            .HasForeignKey(rd => rd.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RoomDescription>()
            .HasOne(rd => rd.RequiredItem)
            .WithMany()
            .HasForeignKey(rd => rd.RequiredItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoomDescription>()
            .HasOne(rd => rd.RequiredAction)
            .WithMany()
            .HasForeignKey(rd => rd.RequiredActionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure VisitedRoom
        modelBuilder.Entity<VisitedRoom>()
            .HasOne(vr => vr.GameSave)
            .WithMany()
            .HasForeignKey(vr => vr.GameSaveId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VisitedRoom>()
            .HasOne(vr => vr.Room)
            .WithMany()
            .HasForeignKey(vr => vr.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VisitedRoom>()
            .HasIndex(vr => new { vr.GameSaveId, vr.RoomId })
            .IsUnique();
    }
}
