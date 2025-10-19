using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddDarkRoomSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDark",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LightSourceItemId",
                table: "Rooms",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_LightSourceItemId",
                table: "Rooms",
                column: "LightSourceItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Items_LightSourceItemId",
                table: "Rooms",
                column: "LightSourceItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Items_LightSourceItemId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_LightSourceItemId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "IsDark",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "LightSourceItemId",
                table: "Rooms");
        }
    }
}
