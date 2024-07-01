using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructApp.Migrations
{
    /// <inheritdoc />
    public partial class AllTables2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMaterials_Materials_MaterialId",
                table: "ProjectMaterials");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMaterials_MaterialId",
                table: "ProjectMaterials");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "ProjectMaterials");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaterialId",
                table: "ProjectMaterials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMaterials_MaterialId",
                table: "ProjectMaterials",
                column: "MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMaterials_Materials_MaterialId",
                table: "ProjectMaterials",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
