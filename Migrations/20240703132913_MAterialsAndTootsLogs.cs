using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructApp.Migrations
{
    /// <inheritdoc />
    public partial class MAterialsAndTootsLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "ProjectMaterialUpdateLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectMaterialId = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Changes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMaterialUpdateLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMaterialUpdateLogs_ProjectMaterials_ProjectMaterialId",
                        column: x => x.ProjectMaterialId,
                        principalTable: "ProjectMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectToolUpdateLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectToolId = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Changes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectToolUpdateLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectToolUpdateLogs_ProjectTools_ProjectToolId",
                        column: x => x.ProjectToolId,
                        principalTable: "ProjectTools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMaterialUpdateLogs_ProjectMaterialId",
                table: "ProjectMaterialUpdateLogs",
                column: "ProjectMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectToolUpdateLogs_ProjectToolId",
                table: "ProjectToolUpdateLogs",
                column: "ProjectToolId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_AspNetUsers_ApproverId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Expenses_ExpenseId",
                table: "Approvals");

            migrationBuilder.DropTable(
                name: "ProjectMaterialUpdateLogs");

            migrationBuilder.DropTable(
                name: "ProjectToolUpdateLogs");

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
    }
}
