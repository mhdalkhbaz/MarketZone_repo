using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketZone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class trip1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoastingInvoiceDetailReceipts_Products_ReadyProductId",
                table: "RoastingInvoiceDetailReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_RoastingInvoiceDetailReceipts_RoastingInvoiceDetails_DetailId",
                table: "RoastingInvoiceDetailReceipts");

            migrationBuilder.DropIndex(
                name: "IX_RoastingInvoiceDetailReceipts_ReadyProductId",
                table: "RoastingInvoiceDetailReceipts");

            migrationBuilder.AddForeignKey(
                name: "FK_RoastingInvoiceDetailReceipts_RoastingInvoiceDetails_DetailId",
                table: "RoastingInvoiceDetailReceipts",
                column: "DetailId",
                principalTable: "RoastingInvoiceDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoastingInvoiceDetailReceipts_RoastingInvoiceDetails_DetailId",
                table: "RoastingInvoiceDetailReceipts");

            migrationBuilder.CreateIndex(
                name: "IX_RoastingInvoiceDetailReceipts_ReadyProductId",
                table: "RoastingInvoiceDetailReceipts",
                column: "ReadyProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoastingInvoiceDetailReceipts_Products_ReadyProductId",
                table: "RoastingInvoiceDetailReceipts",
                column: "ReadyProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoastingInvoiceDetailReceipts_RoastingInvoiceDetails_DetailId",
                table: "RoastingInvoiceDetailReceipts",
                column: "DetailId",
                principalTable: "RoastingInvoiceDetails",
                principalColumn: "Id");
        }
    }
}
