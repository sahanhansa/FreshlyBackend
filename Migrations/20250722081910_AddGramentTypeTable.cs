using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshlyBackendNew.Migrations
{
    /// <inheritdoc />
    public partial class AddGramentTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_LaundryItemServices",
            //    table: "LaundryItemServices");

            //migrationBuilder.AddColumn<Guid>(
            //    name: "GarmentTypeId",
            //    table: "LaundryItemServices",
            //    type: "char(36)",
            //    nullable: false,
            //    defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
            //    collation: "ascii_general_ci");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_LaundryItemServices",
            //    table: "LaundryItemServices",
            //    columns: new[] { "LaundryId", "ItemId", "ServiceId", "GarmentTypeId" });

            migrationBuilder.Sql(
                @"ALTER TABLE `LaundryItemServices`
                  DROP PRIMARY KEY,
                  ADD PRIMARY KEY (`LaundryId`, `ItemId`, `ServiceId`, `GarmentTypeId`);"
            );

            //migrationBuilder.CreateTable(
            //    name: "GarmentTypes",
            //    columns: table => new
            //    {
            //        GarmentTypeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //        GarmentTypeName = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_GarmentTypes", x => x.GarmentTypeId);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql(@"
    CREATE TABLE IF NOT EXISTS `GarmentTypes` (
        `GarmentTypeId` char(36) COLLATE ascii_general_ci NOT NULL,
        `GarmentTypeName` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_GarmentTypes` PRIMARY KEY (`GarmentTypeId`)
    ) CHARACTER SET=utf8mb4;
");


            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 8, 19, 9, 852, DateTimeKind.Utc).AddTicks(5875));

            //migrationBuilder.CreateIndex(
            //    name: "IX_LaundryItemServices_GarmentTypeId",
            //    table: "LaundryItemServices",
            //    column: "GarmentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LaundryItemServices_GarmentTypes_GarmentTypeId",
                table: "LaundryItemServices",
                column: "GarmentTypeId",
                principalTable: "GarmentTypes",
                principalColumn: "GarmentTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaundryItemServices_GarmentTypes_GarmentTypeId",
                table: "LaundryItemServices");

            migrationBuilder.DropTable(
                name: "GarmentTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LaundryItemServices",
                table: "LaundryItemServices");

            migrationBuilder.DropIndex(
                name: "IX_LaundryItemServices_GarmentTypeId",
                table: "LaundryItemServices");

            migrationBuilder.DropColumn(
                name: "GarmentTypeId",
                table: "LaundryItemServices");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LaundryItemServices",
                table: "LaundryItemServices",
                columns: new[] { "LaundryId", "ItemId", "ServiceId" });

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 8, 3, 22, 211, DateTimeKind.Utc).AddTicks(188));
        }
    }
}
