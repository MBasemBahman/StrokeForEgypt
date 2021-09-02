using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StrokeForEgypt.DAL.Migrations
{
    public partial class dmskdmsk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "EventAgenda",
                newName: "ToTime");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "EventAgenda",
                newName: "ShortDescription");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "EventAgenda",
                newName: "ToDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "FromDate",
                table: "EventAgenda",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "FromTime",
                table: "EventAgenda",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "LongDescription",
                table: "EventAgenda",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "sume6053zy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "EventAgenda");

            migrationBuilder.DropColumn(
                name: "FromTime",
                table: "EventAgenda");

            migrationBuilder.DropColumn(
                name: "LongDescription",
                table: "EventAgenda");

            migrationBuilder.RenameColumn(
                name: "ToTime",
                table: "EventAgenda",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "ToDate",
                table: "EventAgenda",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "ShortDescription",
                table: "EventAgenda",
                newName: "Description");

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "agdy1386uz");
        }
    }
}
