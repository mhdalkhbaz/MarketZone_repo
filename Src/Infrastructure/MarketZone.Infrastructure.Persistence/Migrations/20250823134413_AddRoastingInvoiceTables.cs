using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketZone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRoastingInvoiceTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RoastingInvoiceId",
                table: "Payments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoastingInvoices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    PaymentStatus = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoastingInvoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoastingInvoiceDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoastingInvoiceId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    QuantityKg = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 2, nullable: false),
                    RoastPricePerKg = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoastingInvoiceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoastingInvoiceDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoastingInvoiceDetails_RoastingInvoices_RoastingInvoiceId",
                        column: x => x.RoastingInvoiceId,
                        principalTable: "RoastingInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RoastingInvoiceId",
                table: "Payments",
                column: "RoastingInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_RoastingInvoiceDetails_ProductId",
                table: "RoastingInvoiceDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RoastingInvoiceDetails_RoastingInvoiceId",
                table: "RoastingInvoiceDetails",
                column: "RoastingInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_RoastingInvoices_InvoiceNumber",
                table: "RoastingInvoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_RoastingInvoices_RoastingInvoiceId",
                table: "Payments",
                column: "RoastingInvoiceId",
                principalTable: "RoastingInvoices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_RoastingInvoices_RoastingInvoiceId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "RoastingInvoiceDetails");

            migrationBuilder.DropTable(
                name: "RoastingInvoices");

            migrationBuilder.DropIndex(
                name: "IX_Payments_RoastingInvoiceId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RoastingInvoiceId",
                table: "Payments");
        }
    }
}
