using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddPhase3Features : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerContexts");
        }
    }
}
