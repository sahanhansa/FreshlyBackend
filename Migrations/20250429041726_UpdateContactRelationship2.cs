using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshlyBackendNew.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContactRelationship2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Customers_CustomerId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Laundries_LaundryId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Owners_OwnerId",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_CustomerId",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_LaundryId",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_OwnerId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "LaundryId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Contacts");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Customers_ContactId",
                table: "Contacts",
                column: "ContactId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Laundries_ContactId",
                table: "Contacts",
                column: "ContactId",
                principalTable: "Laundries",
                principalColumn: "LaundryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Owners_ContactId",
                table: "Contacts",
                column: "ContactId",
                principalTable: "Owners",
                principalColumn: "OwnerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Customers_ContactId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Laundries_ContactId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Owners_ContactId",
                table: "Contacts");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Contacts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "LaundryId",
                table: "Contacts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Contacts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_CustomerId",
                table: "Contacts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_LaundryId",
                table: "Contacts",
                column: "LaundryId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_OwnerId",
                table: "Contacts",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Customers_CustomerId",
                table: "Contacts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Laundries_LaundryId",
                table: "Contacts",
                column: "LaundryId",
                principalTable: "Laundries",
                principalColumn: "LaundryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Owners_OwnerId",
                table: "Contacts",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "OwnerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
