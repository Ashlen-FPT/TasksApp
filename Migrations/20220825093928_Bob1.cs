using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TasksApp.Migrations
{
    public partial class Bob1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "BobCats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTaskCompleted",
                table: "BobCats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Sign1",
                table: "BobCats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Sign2",
                table: "BobCats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName1",
                table: "BobCats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName2",
                table: "BobCats",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "BobCats");

            migrationBuilder.DropColumn(
                name: "DateTaskCompleted",
                table: "BobCats");

            migrationBuilder.DropColumn(
                name: "Sign1",
                table: "BobCats");

            migrationBuilder.DropColumn(
                name: "Sign2",
                table: "BobCats");

            migrationBuilder.DropColumn(
                name: "UserName1",
                table: "BobCats");

            migrationBuilder.DropColumn(
                name: "UserName2",
                table: "BobCats");
        }
    }
}
