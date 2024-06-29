using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructApp.Migrations
{
    /// <inheritdoc />
    public partial class SupportDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupportiveDocumentPath",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupportiveDocumentPath",
                table: "Expenses");
        }
    }
}
