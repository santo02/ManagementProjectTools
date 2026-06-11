using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniProjectManager.API.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkspaceMembersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMember_Users_UserId",
                table: "WorkspaceMember");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMember_Workspaces_WorkspaceId",
                table: "WorkspaceMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkspaceMember",
                table: "WorkspaceMember");

            migrationBuilder.RenameTable(
                name: "WorkspaceMember",
                newName: "WorkspaceMembers");

            migrationBuilder.RenameIndex(
                name: "IX_WorkspaceMember_WorkspaceId",
                table: "WorkspaceMembers",
                newName: "IX_WorkspaceMembers_WorkspaceId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkspaceMember_UserId",
                table: "WorkspaceMembers",
                newName: "IX_WorkspaceMembers_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkspaceMembers",
                table: "WorkspaceMembers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMembers_Users_UserId",
                table: "WorkspaceMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMembers_Workspaces_WorkspaceId",
                table: "WorkspaceMembers",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMembers_Users_UserId",
                table: "WorkspaceMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMembers_Workspaces_WorkspaceId",
                table: "WorkspaceMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkspaceMembers",
                table: "WorkspaceMembers");

            migrationBuilder.RenameTable(
                name: "WorkspaceMembers",
                newName: "WorkspaceMember");

            migrationBuilder.RenameIndex(
                name: "IX_WorkspaceMembers_WorkspaceId",
                table: "WorkspaceMember",
                newName: "IX_WorkspaceMember_WorkspaceId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkspaceMembers_UserId",
                table: "WorkspaceMember",
                newName: "IX_WorkspaceMember_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkspaceMember",
                table: "WorkspaceMember",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMember_Users_UserId",
                table: "WorkspaceMember",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMember_Workspaces_WorkspaceId",
                table: "WorkspaceMember",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
