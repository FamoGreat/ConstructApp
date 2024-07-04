using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructApp.Migrations
{
    /// <inheritdoc />
    public partial class LogsTablesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_AspNetUsers_ApproverId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Expenses_ExpenseId",
                table: "Approvals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Approvals",
                table: "Approvals");

            migrationBuilder.RenameTable(
                name: "Approvals",
                newName: "Approval");

            migrationBuilder.RenameIndex(
                name: "IX_Approvals_ExpenseId",
                table: "Approval",
                newName: "IX_Approval_ExpenseId");

            migrationBuilder.RenameIndex(
                name: "IX_Approvals_ApproverId",
                table: "Approval",
                newName: "IX_Approval_ApproverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Approval",
                table: "Approval",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Approval_AspNetUsers_ApproverId",
                table: "Approval",
                column: "ApproverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Approval_Expenses_ExpenseId",
                table: "Approval",
                column: "ExpenseId",
                principalTable: "Expenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approval_AspNetUsers_ApproverId",
                table: "Approval");

            migrationBuilder.DropForeignKey(
                name: "FK_Approval_Expenses_ExpenseId",
                table: "Approval");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Approval",
                table: "Approval");

            migrationBuilder.RenameTable(
                name: "Approval",
                newName: "Approvals");

            migrationBuilder.RenameIndex(
                name: "IX_Approval_ExpenseId",
                table: "Approvals",
                newName: "IX_Approvals_ExpenseId");

            migrationBuilder.RenameIndex(
                name: "IX_Approval_ApproverId",
                table: "Approvals",
                newName: "IX_Approvals_ApproverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Approvals",
                table: "Approvals",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_AspNetUsers_ApproverId",
                table: "Approvals",
                column: "ApproverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Expenses_ExpenseId",
                table: "Approvals",
                column: "ExpenseId",
                principalTable: "Expenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
