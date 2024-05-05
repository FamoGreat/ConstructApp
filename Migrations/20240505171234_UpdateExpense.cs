using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_AspNetUsers_RequesterId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_RequesterId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "RequesterId",
                table: "Expenses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequesterId",
                table: "Expenses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_RequesterId",
                table: "Expenses",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_AspNetUsers_RequesterId",
                table: "Expenses",
                column: "RequesterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
