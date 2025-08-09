using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshlyBackendNew.Migrations
{
    /// <inheritdoc />
    public partial class RemovedMaterial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_RejectedItems_Items_ServiceId",
            //    table: "RejectedItems");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 7, 50, 2, 803, DateTimeKind.Utc).AddTicks(4090));

            migrationBuilder.AddForeignKey(
                name: "FK_RejectedItems_Services_ServiceId",
                table: "RejectedItems",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RejectedItems_Services_ServiceId",
                table: "RejectedItems");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 21, 20, 18, 37, 951, DateTimeKind.Utc).AddTicks(1774));

            //migrationBuilder.AddForeignKey(
            //    name: "FK_RejectedItems_Items_ServiceId",
            //    table: "RejectedItems",
            //    column: "ServiceId",
            //    principalTable: "Items",
            //    principalColumn: "ItemId");
        }
    }
}
