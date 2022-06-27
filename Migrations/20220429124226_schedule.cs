using Microsoft.EntityFrameworkCore.Migrations;

namespace TasksApp.Migrations
{
    public partial class schedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Schedule",
                table: "PreTasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "PreTasks");
        }
    }
}
