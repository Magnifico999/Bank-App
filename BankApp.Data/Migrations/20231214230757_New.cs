using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankApp.Migrations
{
    public partial class New : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateLastUpdated" },
                values: new object[] { new DateTime(2023, 12, 15, 0, 7, 57, 224, DateTimeKind.Local).AddTicks(6970), new DateTime(2023, 12, 15, 0, 7, 57, 224, DateTimeKind.Local).AddTicks(6987) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateLastUpdated" },
                values: new object[] { new DateTime(2023, 12, 14, 23, 32, 40, 521, DateTimeKind.Local).AddTicks(1441), new DateTime(2023, 12, 14, 23, 32, 40, 521, DateTimeKind.Local).AddTicks(1454) });
        }
    }
}
