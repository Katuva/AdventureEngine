using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureEngine.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVocabularyUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vocabularies_Word",
                table: "Vocabularies");

            migrationBuilder.CreateIndex(
                name: "IX_Vocabularies_Word_WordType",
                table: "Vocabularies",
                columns: new[] { "Word", "WordType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vocabularies_Word_WordType",
                table: "Vocabularies");

            migrationBuilder.CreateIndex(
                name: "IX_Vocabularies_Word",
                table: "Vocabularies",
                column: "Word",
                unique: true);
        }
    }
}
