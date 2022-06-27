using Microsoft.EntityFrameworkCore.Migrations;

namespace TasksApp.Migrations
{
    public partial class annual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Annual",
                table: "TemplateTasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Annual",
                table: "TemplateTasks");
        }
    }
}
