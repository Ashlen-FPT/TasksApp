using Microsoft.EntityFrameworkCore.Migrations;

namespace TasksApp.Migrations
{
    public partial class TTYpe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaskType",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskType",
                table: "PreTasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskType",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskType",
                table: "PreTasks");
        }
    }
}
