using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TasksApp.Migrations
{
    public partial class db1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAllTaskCompleted",
                table: "Maintenances");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAllCompleted",
                table: "Maintenances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAllTaskCompleted",
                table: "items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "items",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAllCompleted",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "DateAllTaskCompleted",
                table: "items");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "items");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAllTaskCompleted",
                table: "Maintenances",
                type: "datetime2",
                nullable: true);
        }
    }
}
