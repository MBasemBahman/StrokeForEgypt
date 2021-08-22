using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace StrokeForEgypt.DAL.Migrations
{
    public partial class newedits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountDevice_NotificationToken",
                table: "AccountDevice");

            migrationBuilder.DropIndex(
                name: "IX_Account_Token",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "LastActive",
                table: "AccountDevice");

            migrationBuilder.DropColumn(
                name: "LastActive",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "LoginToken",
                table: "Account",
                newName: "LoginTokenHash");

            migrationBuilder.AlterColumn<string>(
                name: "NotificationToken",
                table: "AccountDevice",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "nrlm5012ya");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LoginTokenHash",
                table: "Account",
                newName: "LoginToken");

            migrationBuilder.AlterColumn<string>(
                name: "NotificationToken",
                table: "AccountDevice",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActive",
                table: "AccountDevice",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getutcdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActive",
                table: "Account",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getutcdate()");

            migrationBuilder.AddColumn<Guid>(
                name: "Token",
                table: "Account",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "newid()");

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "stiw5983zg");

            migrationBuilder.CreateIndex(
                name: "IX_AccountDevice_NotificationToken",
                table: "AccountDevice",
                column: "NotificationToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_Token",
                table: "Account",
                column: "Token",
                unique: true);
        }
    }
}
