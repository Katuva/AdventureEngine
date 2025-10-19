using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddLookDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LookDescription",
                table: "ExaminableObjects",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LookDescription",
                table: "ExaminableObjects");
        }
    }
}
