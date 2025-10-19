using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddContainerSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Containers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Keywords = table.Column<string>(type: "TEXT", nullable: true),
                    OpenDescription = table.Column<string>(type: "TEXT", nullable: true),
                    EmptyDescription = table.Column<string>(type: "TEXT", nullable: true),
                    StartsOpen = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsLockable = table.Column<bool>(type: "INTEGER", nullable: false),
                    StartsLocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    KeyItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    UnlockMessage = table.Column<string>(type: "TEXT", nullable: true),
                    LockedMessage = table.Column<string>(type: "TEXT", nullable: true),
                    ShowInRoomDescription = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Containers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Containers_Items_KeyItemId",
                        column: x => x.KeyItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Containers_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ContainerItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContainerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlacedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerItems_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContainerItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContainerStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContainerId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsOpen = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsLocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerStates_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContainerStates_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContainerItems_ContainerId",
                table: "ContainerItems",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerItems_ItemId",
                table: "ContainerItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_KeyItemId",
                table: "Containers",
                column: "KeyItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_RoomId",
                table: "Containers",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerStates_ContainerId",
                table: "ContainerStates",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerStates_GameSaveId_ContainerId",
                table: "ContainerStates",
                columns: new[] { "GameSaveId", "ContainerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContainerItems");

            migrationBuilder.DropTable(
                name: "ContainerStates");

            migrationBuilder.DropTable(
                name: "Containers");
        }
    }
}
