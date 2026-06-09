using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniProjectManager.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToWorkspace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "userId",
                table: "Workspaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userId",
                table: "Workspaces");
        }
    }
}
