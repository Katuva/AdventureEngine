using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddContainerRevealSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Containers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RevealMessage",
                table: "Containers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RevealedByExaminableId",
                table: "Containers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContainerRevealed",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContainerId = table.Column<int>(type: "INTEGER", nullable: false),
                    RevealedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerRevealed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerRevealed_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContainerRevealed_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Containers_RevealedByExaminableId",
                table: "Containers",
                column: "RevealedByExaminableId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerRevealed_ContainerId",
                table: "ContainerRevealed",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerRevealed_GameSaveId_ContainerId",
                table: "ContainerRevealed",
                columns: new[] { "GameSaveId", "ContainerId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Containers_ExaminableObjects_RevealedByExaminableId",
                table: "Containers",
                column: "RevealedByExaminableId",
                principalTable: "ExaminableObjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Containers_ExaminableObjects_RevealedByExaminableId",
                table: "Containers");

            migrationBuilder.DropTable(
                name: "ContainerRevealed");

            migrationBuilder.DropIndex(
                name: "IX_Containers_RevealedByExaminableId",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "RevealMessage",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "RevealedByExaminableId",
                table: "Containers");
        }
    }
}
