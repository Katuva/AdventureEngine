using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddExaminableObjectDisplayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ExaminableObjects",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ExaminableObjects");
        }
    }
}
