using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddHealingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmptyDescription",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HealingAmount",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxUses",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmptyDescription",
                table: "ExaminableObjects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HealingAmount",
                table: "ExaminableObjects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxUses",
                table: "ExaminableObjects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ExaminableObjectUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExaminableObjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsesCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExaminableObjectUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExaminableObjectUsages_ExaminableObjects_ExaminableObjectId",
                        column: x => x.ExaminableObjectId,
                        principalTable: "ExaminableObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExaminableObjectUsages_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsesCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemUsages_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemUsages_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjectUsages_ExaminableObjectId",
                table: "ExaminableObjectUsages",
                column: "ExaminableObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjectUsages_GameSaveId_ExaminableObjectId",
                table: "ExaminableObjectUsages",
                columns: new[] { "GameSaveId", "ExaminableObjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemUsages_GameSaveId_ItemId",
                table: "ItemUsages",
                columns: new[] { "GameSaveId", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemUsages_ItemId",
                table: "ItemUsages",
                column: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExaminableObjectUsages");

            migrationBuilder.DropTable(
                name: "ItemUsages");

            migrationBuilder.DropColumn(
                name: "EmptyDescription",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "HealingAmount",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MaxUses",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "EmptyDescription",
                table: "ExaminableObjects");

            migrationBuilder.DropColumn(
                name: "HealingAmount",
                table: "ExaminableObjects");

            migrationBuilder.DropColumn(
                name: "MaxUses",
                table: "ExaminableObjects");
        }
    }
}
