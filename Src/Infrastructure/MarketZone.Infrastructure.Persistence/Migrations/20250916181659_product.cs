using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketZone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoastingInvoiceDetails_Products_ProductId",
                table: "RoastingInvoiceDetails");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "RoastingInvoiceDetails",
                newName: "ReadyProductId");

            migrationBuilder.RenameIndex(
                name: "IX_RoastingInvoiceDetails_ProductId",
                table: "RoastingInvoiceDetails",
                newName: "IX_RoastingInvoiceDetails_ReadyProductId");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalValue",
                table: "UnroastedProdcutBalances",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPerKg",
                table: "RoastingInvoiceDetails",
                type: "decimal(18,6)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "RawProductId",
                table: "RoastingInvoiceDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPerKg",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RawProductId",
                table: "Products",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalValue",
                table: "ProductBalances",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_RoastingInvoiceDetails_RawProductId",
                table: "RoastingInvoiceDetails",
                column: "RawProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_RawProductId",
                table: "Products",
                column: "RawProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Products_RawProductId",
                table: "Products",
                column: "RawProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoastingInvoiceDetails_Products_RawProductId",
                table: "RoastingInvoiceDetails",
                column: "RawProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoastingInvoiceDetails_Products_ReadyProductId",
                table: "RoastingInvoiceDetails",
                column: "ReadyProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Products_RawProductId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_RoastingInvoiceDetails_Products_RawProductId",
                table: "RoastingInvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_RoastingInvoiceDetails_Products_ReadyProductId",
                table: "RoastingInvoiceDetails");

            migrationBuilder.DropIndex(
                name: "IX_RoastingInvoiceDetails_RawProductId",
                table: "RoastingInvoiceDetails");

            migrationBuilder.DropIndex(
                name: "IX_Products_RawProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TotalValue",
                table: "UnroastedProdcutBalances");

            migrationBuilder.DropColumn(
                name: "CommissionPerKg",
                table: "RoastingInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "RawProductId",
                table: "RoastingInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "CommissionPerKg",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RawProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TotalValue",
                table: "ProductBalances");

            migrationBuilder.RenameColumn(
                name: "ReadyProductId",
                table: "RoastingInvoiceDetails",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_RoastingInvoiceDetails_ReadyProductId",
                table: "RoastingInvoiceDetails",
                newName: "IX_RoastingInvoiceDetails_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoastingInvoiceDetails_Products_ProductId",
                table: "RoastingInvoiceDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
