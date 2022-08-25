using Microsoft.EntityFrameworkCore.Migrations;

namespace TasksApp.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyChecksSubs_TemplateItem_MainId",
                table: "DailyChecksSubs");

            migrationBuilder.AlterColumn<int>(
                name: "MainId",
                table: "DailyChecksSubs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyChecksSubs_TemplateItem_MainId",
                table: "DailyChecksSubs",
                column: "MainId",
                principalTable: "TemplateItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyChecksSubs_TemplateItem_MainId",
                table: "DailyChecksSubs");

            migrationBuilder.AlterColumn<int>(
                name: "MainId",
                table: "DailyChecksSubs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyChecksSubs_TemplateItem_MainId",
                table: "DailyChecksSubs",
                column: "MainId",
                principalTable: "TemplateItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
