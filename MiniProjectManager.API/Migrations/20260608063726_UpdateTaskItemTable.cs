using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniProjectManager.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Workspaces_WorkspaceId",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_WorkspaceId",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "WorkspaceId",
                table: "TaskItems");

            migrationBuilder.AddColumn<int>(
                name: "AssigneeId",
                table: "TaskItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_AssigneeId",
                table: "TaskItems",
                column: "AssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_TaskItems_AssigneeId",
                table: "TaskItems",
                column: "AssigneeId",
                principalTable: "TaskItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_TaskItems_AssigneeId",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_AssigneeId",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "TaskItems");

            migrationBuilder.AddColumn<int>(
                name: "WorkspaceId",
                table: "TaskItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_WorkspaceId",
                table: "TaskItems",
                column: "WorkspaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Workspaces_WorkspaceId",
                table: "TaskItems",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
