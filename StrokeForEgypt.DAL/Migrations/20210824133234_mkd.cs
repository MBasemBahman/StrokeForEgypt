using Microsoft.EntityFrameworkCore.Migrations;

namespace StrokeForEgypt.DAL.Migrations
{
    public partial class mkd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationCodeHash",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "biwd2029nr");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationCodeHash",
                table: "Account");

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "xjhy2348yb");
        }
    }
}
