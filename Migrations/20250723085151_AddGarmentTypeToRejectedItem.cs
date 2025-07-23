using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshlyBackendNew.Migrations
{
    /// <inheritdoc />
    public partial class AddGarmentTypeToRejectedItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GarmentTypeId",
                table: "RejectedItems",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 23, 8, 51, 49, 720, DateTimeKind.Utc).AddTicks(8483));

            migrationBuilder.CreateIndex(
                name: "IX_RejectedItems_GarmentTypeId",
                table: "RejectedItems",
                column: "GarmentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RejectedItems_GarmentTypes_GarmentTypeId",
                table: "RejectedItems",
                column: "GarmentTypeId",
                principalTable: "GarmentTypes",
                principalColumn: "GarmentTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RejectedItems_GarmentTypes_GarmentTypeId",
                table: "RejectedItems");

            migrationBuilder.DropIndex(
                name: "IX_RejectedItems_GarmentTypeId",
                table: "RejectedItems");

            migrationBuilder.DropColumn(
                name: "GarmentTypeId",
                table: "RejectedItems");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 21, 48, 21, 293, DateTimeKind.Utc).AddTicks(4920));
        }
    }
}
