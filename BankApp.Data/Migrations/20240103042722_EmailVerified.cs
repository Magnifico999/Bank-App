using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankApp.Migrations
{
    public partial class EmailVerified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateLastUpdated" },
                values: new object[] { new DateTime(2024, 1, 3, 5, 27, 22, 338, DateTimeKind.Local).AddTicks(8403), new DateTime(2024, 1, 3, 5, 27, 22, 338, DateTimeKind.Local).AddTicks(8416) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateLastUpdated" },
                values: new object[] { new DateTime(2024, 1, 3, 4, 32, 0, 542, DateTimeKind.Local).AddTicks(9401), new DateTime(2024, 1, 3, 4, 32, 0, 542, DateTimeKind.Local).AddTicks(9411) });
        }
    }
}
