using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomProtectionItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProtectionItemId",
                table: "Rooms",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProtectionItemId1",
                table: "Rooms",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ProtectionItemId1",
                table: "Rooms",
                column: "ProtectionItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Items_ProtectionItemId1",
                table: "Rooms",
                column: "ProtectionItemId1",
                principalTable: "Items",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Items_ProtectionItemId1",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_ProtectionItemId1",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ProtectionItemId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ProtectionItemId1",
                table: "Rooms");
        }
    }
}
