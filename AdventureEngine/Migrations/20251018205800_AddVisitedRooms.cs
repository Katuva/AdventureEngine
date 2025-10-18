using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitedRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_VisitedRooms_GameSaveId_RoomId",
                table: "VisitedRooms",
                columns: new[] { "GameSaveId", "RoomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitedRooms_RoomId",
                table: "VisitedRooms",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisitedRooms");
        }
    }
}
