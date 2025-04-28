using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshlyBackendNew.Migrations
{
    /// <inheritdoc />
    public partial class AddressIssueFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Addresses_AddressId1",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_AddressId1",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AddressId1",
                table: "Customers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AddressId1",
                table: "Customers",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AddressId1",
                table: "Customers",
                column: "AddressId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Addresses_AddressId1",
                table: "Customers",
                column: "AddressId1",
                principalTable: "Addresses",
                principalColumn: "AddressId");
        }
    }
}
