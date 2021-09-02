using Microsoft.EntityFrameworkCore.Migrations;

namespace StrokeForEgypt.DAL.Migrations
{
    public partial class dsmsm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sponsor_Event_Fk_Event",
                table: "Sponsor");

            migrationBuilder.AlterColumn<int>(
                name: "Fk_Event",
                table: "Sponsor",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "kaax8399ou");

            migrationBuilder.AddForeignKey(
                name: "FK_Sponsor_Event_Fk_Event",
                table: "Sponsor",
                column: "Fk_Event",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sponsor_Event_Fk_Event",
                table: "Sponsor");

            migrationBuilder.AlterColumn<int>(
                name: "Fk_Event",
                table: "Sponsor",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "jczi9157mo");

            migrationBuilder.AddForeignKey(
                name: "FK_Sponsor_Event_Fk_Event",
                table: "Sponsor",
                column: "Fk_Event",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
