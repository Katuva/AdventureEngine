using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddActivationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivationMessage",
                table: "ExaminableObjects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActivatable",
                table: "ExaminableObjects",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOneTimeUse",
                table: "ExaminableObjects",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RevealsExaminableId",
                table: "ExaminableObjects",
                type: "INTEGER",
                nullable: true);

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
                    table.ForeignKey(
                        name: "FK_ActivatedExaminableObjects_ExaminableObjects_ExaminableObjectId",
                        column: x => x.ExaminableObjectId,
                        principalTable: "ExaminableObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivatedExaminableObjects_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjects_RevealsExaminableId",
                table: "ExaminableObjects",
                column: "RevealsExaminableId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivatedExaminableObjects_ExaminableObjectId",
                table: "ActivatedExaminableObjects",
                column: "ExaminableObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivatedExaminableObjects_GameSaveId_ExaminableObjectId",
                table: "ActivatedExaminableObjects",
                columns: new[] { "GameSaveId", "ExaminableObjectId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExaminableObjects_ExaminableObjects_RevealsExaminableId",
                table: "ExaminableObjects",
                column: "RevealsExaminableId",
                principalTable: "ExaminableObjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExaminableObjects_ExaminableObjects_RevealsExaminableId",
                table: "ExaminableObjects");

            migrationBuilder.DropTable(
                name: "ActivatedExaminableObjects");

            migrationBuilder.DropIndex(
                name: "IX_ExaminableObjects_RevealsExaminableId",
                table: "ExaminableObjects");

            migrationBuilder.DropColumn(
                name: "ActivationMessage",
                table: "ExaminableObjects");

            migrationBuilder.DropColumn(
                name: "IsActivatable",
                table: "ExaminableObjects");

            migrationBuilder.DropColumn(
                name: "IsOneTimeUse",
                table: "ExaminableObjects");

            migrationBuilder.DropColumn(
                name: "RevealsExaminableId",
                table: "ExaminableObjects");
        }
    }
}
