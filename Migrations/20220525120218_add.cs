using Microsoft.EntityFrameworkCore.Migrations;

namespace TasksApp.Migrations
{
    public partial class add : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Annual",
                table: "Main_Task",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bi_Annual",
                table: "Main_Task",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Quarterly",
                table: "Main_Task",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Annual",
                table: "Main_Task");

            migrationBuilder.DropColumn(
                name: "Bi_Annual",
                table: "Main_Task");

            migrationBuilder.DropColumn(
                name: "Quarterly",
                table: "Main_Task");
        }
    }
}
