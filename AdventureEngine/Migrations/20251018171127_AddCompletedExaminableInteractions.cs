using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletedExaminableInteractions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    table.ForeignKey(
                        name: "FK_CompletedExaminableInteractions_ExaminableObjects_ExaminableObjectId",
                        column: x => x.ExaminableObjectId,
                        principalTable: "ExaminableObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompletedExaminableInteractions_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompletedExaminableInteractions_ExaminableObjectId",
                table: "CompletedExaminableInteractions",
                column: "ExaminableObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedExaminableInteractions_GameSaveId",
                table: "CompletedExaminableInteractions",
                column: "GameSaveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompletedExaminableInteractions");
        }
    }
}
