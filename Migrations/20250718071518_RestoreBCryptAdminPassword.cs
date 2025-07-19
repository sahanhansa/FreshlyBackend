using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshlyBackendNew.Migrations
{
    /// <inheritdoc />
    public partial class RestoreBCryptAdminPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 7, 18, 7, 15, 18, 251, DateTimeKind.Utc).AddTicks(9127), "$2a$11$1mN9MtoLb5x./cJJfF9DUOs7O6w2HMInYQ6D.U1OU1Vm4eMjMKmNa" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 7, 18, 6, 41, 19, 911, DateTimeKind.Utc).AddTicks(7707), "admin123" });
        }
    }
}
