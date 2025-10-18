using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddExaminableObjectInteractions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FailureMessage",
                table: "ExaminableObjects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredItemId",
                table: "ExaminableObjects",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuccessMessage",
                table: "ExaminableObjects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnlocksRoomId",
                table: "ExaminableObjects",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjects_RequiredItemId",
                table: "ExaminableObjects",
                column: "RequiredItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminableObjects_UnlocksRoomId",
                table: "ExaminableObjects",
                column: "UnlocksRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExaminableObjects_Items_RequiredItemId",
                table: "ExaminableObjects",
                column: "RequiredItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExaminableObjects_Rooms_UnlocksRoomId",
                table: "ExaminableObjects",
                column: "UnlocksRoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExaminableObjects_Items_RequiredItemId",
                table: "ExaminableObjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ExaminableObjects_Rooms_UnlocksRoomId",
                table: "ExaminableObjects");

            migrationBuilder.DropIndex(
                name: "IX_ExaminableObjects_RequiredItemId",
                table: "ExaminableObjects");

            migrationBuilder.DropIndex(
                name: "IX_ExaminableObjects_UnlocksRoomId",
                table: "ExaminableObjects");

            migrationBuilder.DropColumn(
                name: "FailureMessage",
                table: "ExaminableObjects");

            migrationBuilder.DropColumn(
                name: "RequiredItemId",
                table: "ExaminableObjects");

            migrationBuilder.DropColumn(
                name: "SuccessMessage",
                table: "ExaminableObjects");

            migrationBuilder.DropColumn(
                name: "UnlocksRoomId",
                table: "ExaminableObjects");
        }
    }
}
