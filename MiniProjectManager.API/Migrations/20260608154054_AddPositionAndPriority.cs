using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniProjectManager.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionAndPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_TaskItems_AssigneeId",
                table: "TaskItems");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "TaskItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Boards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Users_AssigneeId",
                table: "TaskItems",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Users_AssigneeId",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Boards");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_TaskItems_AssigneeId",
                table: "TaskItems",
                column: "AssigneeId",
                principalTable: "TaskItems",
                principalColumn: "Id");
        }
    }
}
