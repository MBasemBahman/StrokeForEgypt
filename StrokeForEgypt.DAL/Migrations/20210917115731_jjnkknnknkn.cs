using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace StrokeForEgypt.DAL.Migrations
{
    public partial class jjnkknnknkn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_OpenType_Fk_OpenType",
                table: "Notification");

            migrationBuilder.DropTable(
                name: "OpenType");

            migrationBuilder.DropIndex(
                name: "IX_Notification_Fk_OpenType",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "Fk_OpenType",
                table: "Notification");

            migrationBuilder.RenameColumn(
                name: "Target_Id",
                table: "Notification",
                newName: "Target");

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "sgfy1277wf");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Target",
                table: "Notification",
                newName: "Target_Id");

            migrationBuilder.AddColumn<int>(
                name: "Fk_OpenType",
                table: "Notification",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OpenType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenType", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "xwle6920ya");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Fk_OpenType",
                table: "Notification",
                column: "Fk_OpenType");

            migrationBuilder.CreateIndex(
                name: "IX_OpenType_Name",
                table: "OpenType",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_OpenType_Fk_OpenType",
                table: "Notification",
                column: "Fk_OpenType",
                principalTable: "OpenType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
