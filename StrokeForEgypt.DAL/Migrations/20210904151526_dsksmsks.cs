using Microsoft.EntityFrameworkCore.Migrations;

namespace StrokeForEgypt.DAL.Migrations
{
    public partial class dsksmsks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StayHotel",
                table: "Booking");

            migrationBuilder.AddColumn<bool>(
                name: "StayHotel",
                table: "EventPackage",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "zjon2420fp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StayHotel",
                table: "EventPackage");

            migrationBuilder.AddColumn<bool>(
                name: "StayHotel",
                table: "Booking",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "iiva2207jp");
        }
    }
}
