using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vocabularies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Word = table.Column<string>(type: "TEXT", nullable: false),
                    WordType = table.Column<string>(type: "TEXT", nullable: false),
                    CanonicalForm = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vocabularies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivatedExaminableObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExaminableObjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivatedExaminableObjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompletedActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoomActionId = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompletedExaminableInteractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExaminableObjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedExaminableInteractions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExaminableObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    LookDescription = table.Column<string>(type: "TEXT", nullable: true),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowInRoomDescription = table.Column<bool>(type: "INTEGER", nullable: false),
                    RevealedByActionId = table.Column<int>(type: "INTEGER", nullable: true),
                    RevealedByExaminableId = table.Column<int>(type: "INTEGER", nullable: true),
                    RevealedByItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    RevealMessage = table.Column<string>(type: "TEXT", nullable: true),
                    ShowRevealMessage = table.Column<bool>(type: "INTEGER", nullable: false),
                    Keywords = table.Column<string>(type: "TEXT", nullable: true),
                    RequiredItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    UnlocksRoomId = table.Column<int>(type: "INTEGER", nullable: true),
                    UnlockDirection = table.Column<string>(type: "TEXT", nullable: true),
                    SuccessMessage = table.Column<string>(type: "TEXT", nullable: true),
                    FailureMessage = table.Column<string>(type: "TEXT", nullable: true),
                    IsActivatable = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsOneTimeUse = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActivationMessage = table.Column<string>(type: "TEXT", nullable: true),
                    RevealsExaminableId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExaminableObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExaminableObjects_ExaminableObjects_RevealedByExaminableId",
                        column: x => x.RevealedByExaminableId,
                        principalTable: "ExaminableObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExaminableObjects_ExaminableObjects_RevealsExaminableId",
                        column: x => x.RevealsExaminableId,
                        principalTable: "ExaminableObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameSaves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SlotName = table.Column<string>(type: "TEXT", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CurrentRoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false),
                    Health = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPlayerDead = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSaves", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RevealedExaminableObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExaminableObjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    RevealedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevealedExaminableObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RevealedExaminableObjects_ExaminableObjects_ExaminableObjectId",
                        column: x => x.ExaminableObjectId,
                        principalTable: "ExaminableObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RevealedExaminableObjects_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    PickedUpAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemAdjectives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Adjective = table.Column<string>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAdjectives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    IsCollectable = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsQuestItem = table.Column<bool>(type: "INTEGER", nullable: false),
                    Weight = table.Column<int>(type: "INTEGER", nullable: false),
                    UseMessage = table.Column<string>(type: "TEXT", nullable: true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    State = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemStates_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemStates_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    NorthRoomId = table.Column<int>(type: "INTEGER", nullable: true),
                    SouthRoomId = table.Column<int>(type: "INTEGER", nullable: true),
                    EastRoomId = table.Column<int>(type: "INTEGER", nullable: true),
                    WestRoomId = table.Column<int>(type: "INTEGER", nullable: true),
                    UpRoomId = table.Column<int>(type: "INTEGER", nullable: true),
                    DownRoomId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsStartingRoom = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsWinningRoom = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeadlyRoom = table.Column<bool>(type: "INTEGER", nullable: false),
                    DamageAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    DeathMessage = table.Column<string>(type: "TEXT", nullable: true),
                    WinMessage = table.Column<string>(type: "TEXT", nullable: true),
                    ProtectionItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    ProtectionItemId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    RequiredItemState = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Items_ProtectionItemId1",
                        column: x => x.ProtectionItemId1,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rooms_Rooms_DownRoomId",
                        column: x => x.DownRoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rooms_Rooms_EastRoomId",
                        column: x => x.EastRoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rooms_Rooms_NorthRoomId",
                        column: x => x.NorthRoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rooms_Rooms_SouthRoomId",
                        column: x => x.SouthRoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rooms_Rooms_UpRoomId",
                        column: x => x.UpRoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rooms_Rooms_WestRoomId",
                        column: x => x.WestRoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlacedItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlacedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlacedItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlacedItems_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlacedItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlacedItems_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerContexts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    LastMentionedItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    LastExaminedObjectId = table.Column<int>(type: "INTEGER", nullable: true),
                    LastRoomId = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerContexts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerContexts_ExaminableObjects_LastExaminedObjectId",
                        column: x => x.LastExaminedObjectId,
                        principalTable: "ExaminableObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PlayerContexts_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerContexts_Items_LastMentionedItemId",
                        column: x => x.LastMentionedItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PlayerContexts_Rooms_LastRoomId",
                        column: x => x.LastRoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RoomActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActionName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    RequiredItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    SuccessMessage = table.Column<string>(type: "TEXT", nullable: true),
                    FailureMessage = table.Column<string>(type: "TEXT", nullable: true),
                    UnlocksRoomId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsRepeatable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomActions_Items_RequiredItemId",
                        column: x => x.RequiredItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomActions_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomActions_Rooms_UnlocksRoomId",
                        column: x => x.UnlocksRoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VisitedRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstVisitedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastVisitedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    VisitCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitedRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitedRooms_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitedRooms_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    ConditionType = table.Column<string>(type: "TEXT", nullable: false),
                    RequiredItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    RequiredItemState = table.Column<string>(type: "TEXT", nullable: true),
                    RequiredActionId = table.Column<int>(type: "INTEGER", nullable: true),
                    ActionMustBeCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    ItemMustBeOwned = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomDescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomDescriptions_Items_RequiredItemId",
                        column: x => x.RequiredItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomDescriptions_RoomActions_RequiredActionId",
                        column: x => x.RequiredActionId,
                        principalTable: "RoomActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomDescriptions_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivatedExaminableObjects_ExaminableObjectId",
                table: "ActivatedExaminableObjects",
                column: "ExaminableObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivatedExaminableObjects_GameSaveId_ExaminableObjectId",
                table: "ActivatedExaminableObjects",
                columns: new[] { "GameSaveId", "ExaminableObjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompletedActions_GameSaveId",
                table: "CompletedActions",
                column: "GameSaveId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedActions_RoomActionId",
                table: "CompletedActions",
                column: "RoomActionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedExaminableInteractions_ExaminableObjectId",
                table: "CompletedExaminableInteractions",
                column: "ExaminableObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedExaminableInteractions_GameSaveId",
                table: "CompletedExaminableInteractions",
                column: "GameSaveId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjects_RequiredItemId",
                table: "ExaminableObjects",
                column: "RequiredItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjects_RevealedByActionId",
                table: "ExaminableObjects",
                column: "RevealedByActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjects_RevealedByExaminableId",
                table: "ExaminableObjects",
                column: "RevealedByExaminableId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjects_RevealedByItemId",
                table: "ExaminableObjects",
                column: "RevealedByItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjects_RevealsExaminableId",
                table: "ExaminableObjects",
                column: "RevealsExaminableId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjects_RoomId",
                table: "ExaminableObjects",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjects_UnlocksRoomId",
                table: "ExaminableObjects",
                column: "UnlocksRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSaves_CurrentRoomId",
                table: "GameSaves",
                column: "CurrentRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSaves_SlotName",
                table: "GameSaves",
                column: "SlotName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_GameSaveId",
                table: "InventoryItems",
                column: "GameSaveId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ItemId",
                table: "InventoryItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAdjectives_ItemId_Adjective",
                table: "ItemAdjectives",
                columns: new[] { "ItemId", "Adjective" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_RoomId",
                table: "Items",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemStates_GameSaveId_ItemId",
                table: "ItemStates",
                columns: new[] { "GameSaveId", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemStates_ItemId",
                table: "ItemStates",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PlacedItems_GameSaveId",
                table: "PlacedItems",
                column: "GameSaveId");

            migrationBuilder.CreateIndex(
                name: "IX_PlacedItems_ItemId",
                table: "PlacedItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PlacedItems_RoomId",
                table: "PlacedItems",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerContexts_GameSaveId",
                table: "PlayerContexts",
                column: "GameSaveId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerContexts_LastExaminedObjectId",
                table: "PlayerContexts",
                column: "LastExaminedObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerContexts_LastMentionedItemId",
                table: "PlayerContexts",
                column: "LastMentionedItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerContexts_LastRoomId",
                table: "PlayerContexts",
                column: "LastRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RevealedExaminableObjects_ExaminableObjectId",
                table: "RevealedExaminableObjects",
                column: "ExaminableObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RevealedExaminableObjects_GameSaveId_ExaminableObjectId",
                table: "RevealedExaminableObjects",
                columns: new[] { "GameSaveId", "ExaminableObjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomActions_RequiredItemId",
                table: "RoomActions",
                column: "RequiredItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomActions_RoomId",
                table: "RoomActions",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomActions_UnlocksRoomId",
                table: "RoomActions",
                column: "UnlocksRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomDescriptions_RequiredActionId",
                table: "RoomDescriptions",
                column: "RequiredActionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomDescriptions_RequiredItemId",
                table: "RoomDescriptions",
                column: "RequiredItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomDescriptions_RoomId",
                table: "RoomDescriptions",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_DownRoomId",
                table: "Rooms",
                column: "DownRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_EastRoomId",
                table: "Rooms",
                column: "EastRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_NorthRoomId",
                table: "Rooms",
                column: "NorthRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ProtectionItemId1",
                table: "Rooms",
                column: "ProtectionItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_SouthRoomId",
                table: "Rooms",
                column: "SouthRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_UpRoomId",
                table: "Rooms",
                column: "UpRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_WestRoomId",
                table: "Rooms",
                column: "WestRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitedRooms_GameSaveId_RoomId",
                table: "VisitedRooms",
                columns: new[] { "GameSaveId", "RoomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitedRooms_RoomId",
                table: "VisitedRooms",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Vocabularies_Word_WordType",
                table: "Vocabularies",
                columns: new[] { "Word", "WordType" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivatedExaminableObjects_ExaminableObjects_ExaminableObjectId",
                table: "ActivatedExaminableObjects",
                column: "ExaminableObjectId",
                principalTable: "ExaminableObjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivatedExaminableObjects_GameSaves_GameSaveId",
                table: "ActivatedExaminableObjects",
                column: "GameSaveId",
                principalTable: "GameSaves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedActions_GameSaves_GameSaveId",
                table: "CompletedActions",
                column: "GameSaveId",
                principalTable: "GameSaves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedActions_RoomActions_RoomActionId",
                table: "CompletedActions",
                column: "RoomActionId",
                principalTable: "RoomActions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedExaminableInteractions_ExaminableObjects_ExaminableObjectId",
                table: "CompletedExaminableInteractions",
                column: "ExaminableObjectId",
                principalTable: "ExaminableObjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedExaminableInteractions_GameSaves_GameSaveId",
                table: "CompletedExaminableInteractions",
                column: "GameSaveId",
                principalTable: "GameSaves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExaminableObjects_Items_RequiredItemId",
                table: "ExaminableObjects",
                column: "RequiredItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExaminableObjects_Items_RevealedByItemId",
                table: "ExaminableObjects",
                column: "RevealedByItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExaminableObjects_RoomActions_RevealedByActionId",
                table: "ExaminableObjects",
                column: "RevealedByActionId",
                principalTable: "RoomActions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExaminableObjects_Rooms_RoomId",
                table: "ExaminableObjects",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExaminableObjects_Rooms_UnlocksRoomId",
                table: "ExaminableObjects",
                column: "UnlocksRoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameSaves_Rooms_CurrentRoomId",
                table: "GameSaves",
                column: "CurrentRoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Items_ItemId",
                table: "InventoryItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemAdjectives_Items_ItemId",
                table: "ItemAdjectives",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Rooms_RoomId",
                table: "Items",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Items_ProtectionItemId1",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "ActivatedExaminableObjects");

            migrationBuilder.DropTable(
                name: "CompletedActions");

            migrationBuilder.DropTable(
                name: "CompletedExaminableInteractions");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "ItemAdjectives");

            migrationBuilder.DropTable(
                name: "ItemStates");

            migrationBuilder.DropTable(
                name: "PlacedItems");

            migrationBuilder.DropTable(
                name: "PlayerContexts");

            migrationBuilder.DropTable(
                name: "RevealedExaminableObjects");

            migrationBuilder.DropTable(
                name: "RoomDescriptions");

            migrationBuilder.DropTable(
                name: "VisitedRooms");

            migrationBuilder.DropTable(
                name: "Vocabularies");

            migrationBuilder.DropTable(
                name: "ExaminableObjects");

            migrationBuilder.DropTable(
                name: "GameSaves");

            migrationBuilder.DropTable(
                name: "RoomActions");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
