using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddExaminableObjectRevealSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RevealMessage",
                table: "ExaminableObjects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RevealedByExaminableId",
                table: "ExaminableObjects",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RevealedByItemId",
                table: "ExaminableObjects",
                type: "INTEGER",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjects_RevealedByExaminableId",
                table: "ExaminableObjects",
                column: "RevealedByExaminableId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjects_RevealedByItemId",
                table: "ExaminableObjects",
                column: "RevealedByItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RevealedExaminableObjects_ExaminableObjectId",
                table: "RevealedExaminableObjects",
                column: "ExaminableObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RevealedExaminableObjects_GameSaveId_ExaminableObjectId",
                table: "RevealedExaminableObjects",
                columns: new[] { "GameSaveId", "ExaminableObjectId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExaminableObjects_ExaminableObjects_RevealedByExaminableId",
                table: "ExaminableObjects",
                column: "RevealedByExaminableId",
                principalTable: "ExaminableObjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExaminableObjects_Items_RevealedByItemId",
                table: "ExaminableObjects",
                column: "RevealedByItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExaminableObjects_ExaminableObjects_RevealedByExaminableId",
                table: "ExaminableObjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ExaminableObjects_Items_RevealedByItemId",
                table: "ExaminableObjects");

            migrationBuilder.DropTable(
                name: "RevealedExaminableObjects");

            migrationBuilder.DropIndex(
                name: "IX_ExaminableObjects_RevealedByExaminableId",
                table: "ExaminableObjects");

            migrationBuilder.DropIndex(
                name: "IX_ExaminableObjects_RevealedByItemId",
                table: "ExaminableObjects");

            migrationBuilder.DropColumn(
                name: "RevealMessage",
                table: "ExaminableObjects");

            migrationBuilder.DropColumn(
                name: "RevealedByExaminableId",
                table: "ExaminableObjects");

            migrationBuilder.DropColumn(
                name: "RevealedByItemId",
                table: "ExaminableObjects");
        }
    }
}
