using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TasksApp.Migrations
{
    public partial class aud : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "AffectedColumns",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "NewValues",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "OldValues",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AuditLogs",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "AuditLogs",
                newName: "AreaAccessed");

            migrationBuilder.RenameColumn(
                name: "TableName",
                table: "AuditLogs",
                newName: "ActionName");

            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "AuditLogs",
                newName: "Timestamp");

            migrationBuilder.AddColumn<Guid>(
                name: "AuditID",
                table: "AuditLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                column: "AuditID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "AuditID",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "AuditLogs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "AuditLogs",
                newName: "DateTime");

            migrationBuilder.RenameColumn(
                name: "AreaAccessed",
                table: "AuditLogs",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "ActionName",
                table: "AuditLogs",
                newName: "TableName");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AuditLogs",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "AffectedColumns",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewValues",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldValues",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                column: "Id");
        }
    }
}
