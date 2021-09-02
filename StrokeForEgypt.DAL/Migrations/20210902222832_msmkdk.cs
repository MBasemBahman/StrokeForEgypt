using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace StrokeForEgypt.DAL.Migrations
{
    public partial class msmkdk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_AccountPayment_Fk_AccountPayment",
                table: "Booking");

            migrationBuilder.DropTable(
                name: "AccountPayment");

            migrationBuilder.DropIndex(
                name: "IX_Booking_Fk_AccountPayment",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "Fk_AccountPayment",
                table: "Booking");

            migrationBuilder.CreateTable(
                name: "BookingPayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fk_Booking = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantRefNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderAmount = table.Column<double>(type: "float", nullable: false),
                    PaymentAmount = table.Column<double>(type: "float", nullable: false),
                    FawryFees = table.Column<double>(type: "float", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerMobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerMail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerProfileId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    StatusDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingPayment_Booking_Fk_Booking",
                        column: x => x.Fk_Booking,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "aveq6471sp");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPayment_Fk_Booking",
                table: "BookingPayment",
                column: "Fk_Booking");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingPayment");

            migrationBuilder.AddColumn<int>(
                name: "Fk_AccountPayment",
                table: "Booking",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountPayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerMail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerMobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerProfileId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FawryFees = table.Column<double>(type: "float", nullable: false),
                    Fk_Account = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantRefNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    OrderAmount = table.Column<double>(type: "float", nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentAmount = table.Column<double>(type: "float", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    StatusDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountPayment_Account_Fk_Account",
                        column: x => x.Fk_Account,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "zkxr2881on");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Fk_AccountPayment",
                table: "Booking",
                column: "Fk_AccountPayment",
                unique: true,
                filter: "[Fk_AccountPayment] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPayment_Fk_Account",
                table: "AccountPayment",
                column: "Fk_Account");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_AccountPayment_Fk_AccountPayment",
                table: "Booking",
                column: "Fk_AccountPayment",
                principalTable: "AccountPayment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
