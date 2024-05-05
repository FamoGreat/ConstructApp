using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApprove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Approvals_ExpenseId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Approvals");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_ExpenseId",
                table: "Approvals",
                column: "ExpenseId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Approvals_ExpenseId",
                table: "Approvals");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Approvals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_ExpenseId",
                table: "Approvals",
                column: "ExpenseId");
        }
    }
}
