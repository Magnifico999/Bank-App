using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankApp.Migrations
{
    public partial class SeededData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountName", "AccountNumberGenerated", "AccountType", "CurrentAccountBalance", "DateCreated", "DateLastUpdated", "Email", "FirstName", "LastName", "PhoneNumber", "PinHash", "PinSalt" },
                values: new object[] { 1, " ", "9053769810", 3, "999999999", new DateTime(2023, 12, 14, 23, 32, 40, 521, DateTimeKind.Local).AddTicks(1441), new DateTime(2023, 12, 14, 23, 32, 40, 521, DateTimeKind.Local).AddTicks(1454), "settlement@youbank.com", "YouBank", "settlement Account", "08035064624", null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
