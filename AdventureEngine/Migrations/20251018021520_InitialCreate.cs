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
                    DeathMessage = table.Column<string>(type: "TEXT", nullable: true),
                    WinMessage = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
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
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPlayerDead = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSaves_Rooms_CurrentRoomId",
                        column: x => x.CurrentRoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    table.ForeignKey(
                        name: "FK_Items_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                    table.ForeignKey(
                        name: "FK_InventoryItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    table.ForeignKey(
                        name: "FK_CompletedActions_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompletedActions_RoomActions_RoomActionId",
                        column: x => x.RoomActionId,
                        principalTable: "RoomActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompletedActions_GameSaveId",
                table: "CompletedActions",
                column: "GameSaveId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedActions_RoomActionId",
                table: "CompletedActions",
                column: "RoomActionId");

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
                name: "IX_Items_RoomId",
                table: "Items",
                column: "RoomId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompletedActions");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "RoomActions");

            migrationBuilder.DropTable(
                name: "GameSaves");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
