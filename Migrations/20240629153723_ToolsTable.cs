using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructApp.Migrations
{
    /// <inheritdoc />
    public partial class ToolsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ToolCost",
                table: "ProjectTools",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ToolDescription",
                table: "ProjectTools",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ToolsQuantity",
                table: "ProjectTools",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MaterialDescription",
                table: "ProjectMaterials",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToolCost",
                table: "ProjectTools");

            migrationBuilder.DropColumn(
                name: "ToolDescription",
                table: "ProjectTools");

            migrationBuilder.DropColumn(
                name: "ToolsQuantity",
                table: "ProjectTools");

            migrationBuilder.DropColumn(
                name: "MaterialDescription",
                table: "ProjectMaterials");
        }
    }
}
