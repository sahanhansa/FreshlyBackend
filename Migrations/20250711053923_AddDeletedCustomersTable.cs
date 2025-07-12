using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshlyBackendNew.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedCustomersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Owners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Owners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Feedbacks",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "LaundryId",
                table: "Feedbacks",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "DeletedCustomers",
                columns: table => new
                {
                    CustomerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FirstName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Username = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AddressId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DeletionReason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedCustomers", x => x.CustomerId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_CustomerId",
                table: "Feedbacks",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_LaundryId",
                table: "Feedbacks",
                column: "LaundryId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Customers_UserId",
                table: "Contacts",
                column: "UserId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Customers_CustomerId",
                table: "Feedbacks",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Laundries_LaundryId",
                table: "Feedbacks",
                column: "LaundryId",
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
                name: "FK_Feedbacks_Customers_CustomerId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Laundries_LaundryId",
                table: "Feedbacks");

            migrationBuilder.DropTable(
                name: "DeletedCustomers");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_CustomerId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_LaundryId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "LaundryId",
                table: "Feedbacks");
        }
    }
}
