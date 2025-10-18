using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddExaminableObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExaminableObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    RevealedByActionId = table.Column<int>(type: "INTEGER", nullable: true),
                    Keywords = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExaminableObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExaminableObjects_RoomActions_RevealedByActionId",
                        column: x => x.RevealedByActionId,
                        principalTable: "RoomActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExaminableObjects_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjects_RevealedByActionId",
                table: "ExaminableObjects",
                column: "RevealedByActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjects_RoomId",
                table: "ExaminableObjects",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExaminableObjects");
        }
    }
}
