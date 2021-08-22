using Microsoft.EntityFrameworkCore.Migrations;

namespace StrokeForEgypt.DAL.Migrations
{
    public partial class newupdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "OriginalPrice",
                table: "EventPackage",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "ynvi3773mt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "EventPackage");

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "nrlm5012ya");
        }
    }
}
