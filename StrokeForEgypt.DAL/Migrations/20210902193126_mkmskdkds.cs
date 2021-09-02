using Microsoft.EntityFrameworkCore.Migrations;

namespace StrokeForEgypt.DAL.Migrations
{
    public partial class mkmskdkds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Event_Fk_Event",
                table: "News");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "News",
                newName: "ShortDescription");

            migrationBuilder.AlterColumn<int>(
                name: "Fk_Event",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LongDescription",
                table: "News",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "jczi9157mo");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Event_Fk_Event",
                table: "News",
                column: "Fk_Event",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Event_Fk_Event",
                table: "News");

            migrationBuilder.DropColumn(
                name: "LongDescription",
                table: "News");

            migrationBuilder.RenameColumn(
                name: "ShortDescription",
                table: "News",
                newName: "Description");

            migrationBuilder.AlterColumn<int>(
                name: "Fk_Event",
                table: "News",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "fisg2033jq");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Event_Fk_Event",
                table: "News",
                column: "Fk_Event",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
