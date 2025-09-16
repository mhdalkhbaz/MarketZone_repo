using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketZone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ts1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoastingInvoiceDetails_Products_RawProductId",
                table: "RoastingInvoiceDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_RoastingInvoiceDetails_Products_RawProductId",
                table: "RoastingInvoiceDetails",
                column: "RawProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoastingInvoiceDetails_Products_RawProductId",
                table: "RoastingInvoiceDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_RoastingInvoiceDetails_Products_RawProductId",
                table: "RoastingInvoiceDetails",
                column: "RawProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
