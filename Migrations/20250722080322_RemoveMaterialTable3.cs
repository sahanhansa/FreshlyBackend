using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace FreshlyBackendNew.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMaterialTable3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove MaterialId column from LaundryItemServices
            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "LaundryItemServices"
            );

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 8, 3, 22, 211, DateTimeKind.Utc).AddTicks(188));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 8, 0, 43, 179, DateTimeKind.Utc).AddTicks(2250));
        }
    }
}
