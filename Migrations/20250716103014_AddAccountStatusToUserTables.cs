using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshlyBackendNew.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountStatusToUserTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountStatus",
                table: "Laundries",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AccountStatus",
                table: "Drivers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AccountStatus",
                table: "Customers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 7, 16, 10, 30, 12, 626, DateTimeKind.Utc).AddTicks(3482), "$2a$11$1mN9MtoLb5x./cJJfF9DUOs7O6w2HMInYQ6D.U1OU1Vm4eMjMKmNa" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountStatus",
                table: "Laundries");

            migrationBuilder.DropColumn(
                name: "AccountStatus",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "AccountStatus",
                table: "Customers");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 7, 13, 11, 22, 34, 739, DateTimeKind.Utc).AddTicks(1871), "admin123" });
        }
    }
}
