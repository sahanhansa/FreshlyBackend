using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshlyBackendNew.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTemporaryOrderDetailCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add the new GarmentTypeId columns first
            //migrationBuilder.AddColumn<Guid>(
            //    name: "GarmentTypeId",
            //    table: "TemporaryOrderDetails",
            //    type: "char(36)",
            //    nullable: false,
            //    defaultValue: Guid.Empty,
            //    collation: "ascii_general_ci");

            //migrationBuilder.AddColumn<Guid>(
            //    name: "GarmentTypeId",
            //    table: "OrderDetails",
            //    type: "char(36)",
            //    nullable: false,
            //    defaultValue: Guid.Empty,
            //    collation: "ascii_general_ci");

            // Then drop and add primary keys in one atomic operation using raw SQL
            migrationBuilder.Sql(@"
        ALTER TABLE `TemporaryOrderDetails`
        DROP PRIMARY KEY,
        ADD PRIMARY KEY (`TemporaryOrderId`, `ItemId`, `ServiceId`, `GarmentTypeId`);
    ");

            migrationBuilder.Sql(@"
        ALTER TABLE `OrderDetails`
        DROP PRIMARY KEY,
        ADD PRIMARY KEY (`OrderId`, `ItemId`, `ServiceId`, `GarmentTypeId`);
    ");

            // Create other tables and indexes normally
            //migrationBuilder.CreateTable(
            //    name: "PasswordResetRequests",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        Email = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Code = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //        UserType = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PasswordResetRequests", x => x.Id);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 21, 48, 21, 293, DateTimeKind.Utc).AddTicks(4920));

            //migrationBuilder.CreateIndex(
            //    name: "IX_TemporaryOrderDetails_GarmentTypeId",
            //    table: "TemporaryOrderDetails",
            //    column: "GarmentTypeId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderDetails_GarmentTypeId",
            //    table: "OrderDetails",
            //    column: "GarmentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_GarmentTypes_GarmentTypeId",
                table: "OrderDetails",
                column: "GarmentTypeId",
                principalTable: "GarmentTypes",
                principalColumn: "GarmentTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TemporaryOrderDetails_GarmentTypes_GarmentTypeId",
                table: "TemporaryOrderDetails",
                column: "GarmentTypeId",
                principalTable: "GarmentTypes",
                principalColumn: "GarmentTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys and indexes first
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_GarmentTypes_GarmentTypeId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_TemporaryOrderDetails_GarmentTypes_GarmentTypeId",
                table: "TemporaryOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_TemporaryOrderDetails_GarmentTypeId",
                table: "TemporaryOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_GarmentTypeId",
                table: "OrderDetails");

            // Drop PasswordResetRequests table
            //migrationBuilder.DropTable(
            //    name: "PasswordResetRequests");

            // Drop the columns first
            migrationBuilder.DropColumn(
                name: "GarmentTypeId",
                table: "TemporaryOrderDetails");

            migrationBuilder.DropColumn(
                name: "GarmentTypeId",
                table: "OrderDetails");

            // Restore primary keys in one atomic operation using raw SQL
            migrationBuilder.Sql(@"
        ALTER TABLE `TemporaryOrderDetails`
        DROP PRIMARY KEY,
        ADD PRIMARY KEY (`TemporaryOrderId`, `ItemId`, `ServiceId`);
    ");

            migrationBuilder.Sql(@"
        ALTER TABLE `OrderDetails`
        DROP PRIMARY KEY,
        ADD PRIMARY KEY (`OrderId`, `ItemId`, `ServiceId`);
    ");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 8, 19, 9, 852, DateTimeKind.Utc).AddTicks(5875));
        }

    }
}
