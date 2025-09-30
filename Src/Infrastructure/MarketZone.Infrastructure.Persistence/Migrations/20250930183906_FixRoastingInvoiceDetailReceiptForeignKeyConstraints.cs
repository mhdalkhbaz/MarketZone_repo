using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketZone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixRoastingInvoiceDetailReceiptForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoastingInvoiceDetailReceipts_RoastingInvoices_RoastingInvoiceId",
                table: "RoastingInvoiceDetailReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_RoastingInvoiceDetailReceipts_RoastingInvoices_RoastingInvoiceId1",
                table: "RoastingInvoiceDetailReceipts");

            migrationBuilder.DropIndex(
                name: "IX_RoastingInvoiceDetailReceipts_RoastingInvoiceId1",
                table: "RoastingInvoiceDetailReceipts");

            migrationBuilder.DropColumn(
                name: "RoastingInvoiceId1",
                table: "RoastingInvoiceDetailReceipts");

            migrationBuilder.AddForeignKey(
                name: "FK_RoastingInvoiceDetailReceipts_RoastingInvoices_RoastingInvoiceId",
                table: "RoastingInvoiceDetailReceipts",
                column: "RoastingInvoiceId",
                principalTable: "RoastingInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoastingInvoiceDetailReceipts_RoastingInvoices_RoastingInvoiceId",
                table: "RoastingInvoiceDetailReceipts");

            migrationBuilder.AddColumn<long>(
                name: "RoastingInvoiceId1",
                table: "RoastingInvoiceDetailReceipts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoastingInvoiceDetailReceipts_RoastingInvoiceId1",
                table: "RoastingInvoiceDetailReceipts",
                column: "RoastingInvoiceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RoastingInvoiceDetailReceipts_RoastingInvoices_RoastingInvoiceId",
                table: "RoastingInvoiceDetailReceipts",
                column: "RoastingInvoiceId",
                principalTable: "RoastingInvoices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoastingInvoiceDetailReceipts_RoastingInvoices_RoastingInvoiceId1",
                table: "RoastingInvoiceDetailReceipts",
                column: "RoastingInvoiceId1",
                principalTable: "RoastingInvoices",
                principalColumn: "Id");
        }
    }
}
