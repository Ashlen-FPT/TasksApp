using Microsoft.EntityFrameworkCore.Migrations;

namespace TasksApp.Migrations
{
    public partial class Bob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Brake_Test",
                table: "TemplateBobCat",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "General",
                table: "TemplateBobCat",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Results",
                table: "TemplateBobCat",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Test_Brake",
                table: "TemplateBobCat",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brake_Test",
                table: "TemplateBobCat");

            migrationBuilder.DropColumn(
                name: "General",
                table: "TemplateBobCat");

            migrationBuilder.DropColumn(
                name: "Results",
                table: "TemplateBobCat");

            migrationBuilder.DropColumn(
                name: "Test_Brake",
                table: "TemplateBobCat");
        }
    }
}
