using Microsoft.EntityFrameworkCore.Migrations;

namespace TasksApp.Migrations
{
    public partial class main1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Main",
                table: "TemplateItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MainId",
                table: "DailyChecksSubs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyChecksSubs_MainId",
                table: "DailyChecksSubs",
                column: "MainId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyChecksSubs_TemplateItem_MainId",
                table: "DailyChecksSubs",
                column: "MainId",
                principalTable: "TemplateItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyChecksSubs_TemplateItem_MainId",
                table: "DailyChecksSubs");

            migrationBuilder.DropIndex(
                name: "IX_DailyChecksSubs_MainId",
                table: "DailyChecksSubs");

            migrationBuilder.DropColumn(
                name: "Main",
                table: "TemplateItem");

            migrationBuilder.DropColumn(
                name: "MainId",
                table: "DailyChecksSubs");
        }
    }
}
