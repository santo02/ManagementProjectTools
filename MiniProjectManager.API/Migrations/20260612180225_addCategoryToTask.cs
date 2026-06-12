using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniProjectManager.API.Migrations
{
    /// <inheritdoc />
    public partial class addCategoryToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "TaskItems",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "TaskItems");
        }
    }
}
