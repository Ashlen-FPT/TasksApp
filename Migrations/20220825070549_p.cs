using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TasksApp.Migrations
{
    public partial class p : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "DailyChecksSubs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Main",
                table: "DailyChecksSubs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Schedule",
                table: "DailyChecksSubs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "DailyChecksSubs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "DailyChecksSubs");

            migrationBuilder.DropColumn(
                name: "Main",
                table: "DailyChecksSubs");

            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "DailyChecksSubs");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "DailyChecksSubs");
        }
    }
}
