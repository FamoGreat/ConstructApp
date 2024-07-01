using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructApp.Migrations
{
    /// <inheritdoc />
    public partial class ProjectCosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalMaterialExpense",
                table: "Projects",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalToolExpense",
                table: "Projects",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalMaterialExpense",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TotalToolExpense",
                table: "Projects");
        }
    }
}
