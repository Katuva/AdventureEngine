using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class RenameDisappearsWhenDepletedToDisappearsWhenEmpty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DisappearsWhenDepleted",
                table: "Items",
                newName: "DisappearsWhenEmpty");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DisappearsWhenEmpty",
                table: "Items",
                newName: "DisappearsWhenDepleted");
        }
    }
}
