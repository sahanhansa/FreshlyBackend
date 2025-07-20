using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshlyBackendNew.Migrations
{
    /// <inheritdoc />
    public partial class Abcfd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Contacts_Customers_CustomerId",
            //    table: "Contacts");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Contacts_Drivers_DriverId",
            //    table: "Contacts");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Contacts_Laundries_LaundryId",
            //    table: "Contacts");

            //migrationBuilder.DropIndex(
            //    name: "IX_Contacts_CustomerId",
            //    table: "Contacts");

            //migrationBuilder.DropIndex(
            //    name: "IX_Contacts_DriverId",
            //    table: "Contacts");

            //migrationBuilder.DropIndex(
            //    name: "IX_Contacts_LaundryId",
            //    table: "Contacts");

            //migrationBuilder.DropColumn(
            //    name: "CustomerId",
            //    table: "Contacts");

            //migrationBuilder.DropColumn(
            //    name: "DriverId",
            //    table: "Contacts");

            //migrationBuilder.DropColumn(
            //    name: "LaundryId",
            //    table: "Contacts");

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "UserType",
                keyValue: null,
                column: "UserType",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserType",
                table: "Contacts",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Contacts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ContactNumber",
                table: "Contacts",
                type: "varchar(20)",
                nullable: true,
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
                value: new DateTime(2025, 7, 20, 13, 1, 30, 964, DateTimeKind.Utc).AddTicks(911));

            //migrationBuilder.CreateIndex(
            //    name: "IX_Contacts_UserId",
            //    table: "Contacts",
            //    column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Customers_UserId",
                table: "Contacts",
                column: "UserId",
                principalTable: "Customers",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Drivers_UserId",
                table: "Contacts",
                column: "UserId",
                principalTable: "Drivers",
                principalColumn: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Laundries_UserId",
                table: "Contacts",
                column: "UserId",
                principalTable: "Laundries",
                principalColumn: "LaundryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Customers_UserId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Drivers_UserId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Laundries_UserId",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts");

            migrationBuilder.AlterColumn<string>(
                name: "UserType",
                table: "Contacts",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Contacts",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ContactNumber",
                table: "Contacts",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Contacts",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "DriverId",
                table: "Contacts",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "LaundryId",
                table: "Contacts",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 20, 12, 56, 41, 621, DateTimeKind.Utc).AddTicks(930));

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_CustomerId",
                table: "Contacts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_DriverId",
                table: "Contacts",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_LaundryId",
                table: "Contacts",
                column: "LaundryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Customers_CustomerId",
                table: "Contacts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Drivers_DriverId",
                table: "Contacts",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Laundries_LaundryId",
                table: "Contacts",
                column: "LaundryId",
                principalTable: "Laundries",
                principalColumn: "LaundryId");
        }
    }
}
