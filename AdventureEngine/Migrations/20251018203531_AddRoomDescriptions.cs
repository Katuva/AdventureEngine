using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomDescriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    ConditionType = table.Column<string>(type: "TEXT", nullable: false),
                    RequiredItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    RequiredItemState = table.Column<string>(type: "TEXT", nullable: true),
                    RequiredActionId = table.Column<int>(type: "INTEGER", nullable: true),
                    ActionMustBeCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    ItemMustBeOwned = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomDescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomDescriptions_Items_RequiredItemId",
                        column: x => x.RequiredItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomDescriptions_RoomActions_RequiredActionId",
                        column: x => x.RequiredActionId,
                        principalTable: "RoomActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomDescriptions_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomDescriptions_RequiredActionId",
                table: "RoomDescriptions",
                column: "RequiredActionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomDescriptions_RequiredItemId",
                table: "RoomDescriptions",
                column: "RequiredItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomDescriptions_RoomId",
                table: "RoomDescriptions",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomDescriptions");
        }
    }
}
