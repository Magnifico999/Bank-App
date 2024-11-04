using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankApp.Migrations
{
    public partial class Next : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateLastUpdated" },
                values: new object[] { new DateTime(2023, 12, 16, 4, 9, 17, 14, DateTimeKind.Local).AddTicks(4376), new DateTime(2023, 12, 16, 4, 9, 17, 14, DateTimeKind.Local).AddTicks(4390) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateLastUpdated" },
                values: new object[] { new DateTime(2023, 12, 15, 0, 7, 57, 224, DateTimeKind.Local).AddTicks(6970), new DateTime(2023, 12, 15, 0, 7, 57, 224, DateTimeKind.Local).AddTicks(6987) });
        }
    }
}
