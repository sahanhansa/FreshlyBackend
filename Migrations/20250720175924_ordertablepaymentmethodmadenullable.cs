using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshlyBackendNew.Migrations
{
    /// <inheritdoc />
    public partial class ordertablepaymentmethodmadenullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 20, 17, 59, 24, 342, DateTimeKind.Utc).AddTicks(3077));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "PaymentMethod",
                keyValue: null,
                column: "PaymentMethod",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 20, 17, 44, 31, 295, DateTimeKind.Utc).AddTicks(3932));
        }
    }
}
