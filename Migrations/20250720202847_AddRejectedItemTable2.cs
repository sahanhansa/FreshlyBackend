using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshlyBackendNew.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectedItemTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RejectedItems_Items_ItemId",
                table: "RejectedItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RejectedItems_Items_ServiceId",
                table: "RejectedItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RejectedItems_Laundries_LaundryId",
                table: "RejectedItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RejectedItems_Orders_OrderId",
                table: "RejectedItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceId",
                table: "RejectedItems",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "RejectedItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "RejectedItems",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "LaundryId",
                table: "RejectedItems",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ItemName",
                table: "RejectedItems",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "RejectedItems",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 20, 20, 28, 46, 957, DateTimeKind.Utc).AddTicks(4489));

            migrationBuilder.AddForeignKey(
                name: "FK_RejectedItems_Items_ItemId",
                table: "RejectedItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_RejectedItems_Items_ServiceId",
                table: "RejectedItems",
                column: "ServiceId",
                principalTable: "Items",
                principalColumn: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_RejectedItems_Laundries_LaundryId",
                table: "RejectedItems",
                column: "LaundryId",
                principalTable: "Laundries",
                principalColumn: "LaundryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RejectedItems_Orders_OrderId",
                table: "RejectedItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RejectedItems_Items_ItemId",
                table: "RejectedItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RejectedItems_Items_ServiceId",
                table: "RejectedItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RejectedItems_Laundries_LaundryId",
                table: "RejectedItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RejectedItems_Orders_OrderId",
                table: "RejectedItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceId",
                table: "RejectedItems",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "RejectedItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "RejectedItems",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "LaundryId",
                table: "RejectedItems",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "RejectedItems",
                keyColumn: "ItemName",
                keyValue: null,
                column: "ItemName",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ItemName",
                table: "RejectedItems",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "RejectedItems",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 20, 20, 17, 25, 13, DateTimeKind.Utc).AddTicks(3258));

            migrationBuilder.AddForeignKey(
                name: "FK_RejectedItems_Items_ItemId",
                table: "RejectedItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RejectedItems_Items_ServiceId",
                table: "RejectedItems",
                column: "ServiceId",
                principalTable: "Items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RejectedItems_Laundries_LaundryId",
                table: "RejectedItems",
                column: "LaundryId",
                principalTable: "Laundries",
                principalColumn: "LaundryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RejectedItems_Orders_OrderId",
                table: "RejectedItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
