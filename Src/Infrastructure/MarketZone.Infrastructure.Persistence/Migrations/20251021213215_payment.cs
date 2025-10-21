using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketZone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class payment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExchangeRates_CurrencyCode_EffectiveAtUtc",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "RateToUSD",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "ExchangeRates");

            migrationBuilder.RenameColumn(
                name: "EffectiveAtUtc",
                table: "ExchangeRates",
                newName: "EffectiveDate");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "SalesInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "PurchaseInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountInPaymentCurrency",
                table: "Payments",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "Payments",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "InvoiceType",
                table: "Payments",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentCurrency",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ExchangeRates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Rate",
                table: "ExchangeRates",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExchangeTransactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CashRegisterId = table.Column<long>(type: "bigint", nullable: false),
                    Direction = table.Column<short>(type: "smallint", nullable: false),
                    FromAmount = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    ToAmount = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeTransactions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeTransactions");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "AmountInPaymentCurrency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "InvoiceType",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentCurrency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "EffectiveDate",
                table: "ExchangeRates",
                newName: "EffectiveAtUtc");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "ExchangeRates",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ExchangeRates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "RateToUSD",
                table: "ExchangeRates",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "ExchangeRates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_CurrencyCode_EffectiveAtUtc",
                table: "ExchangeRates",
                columns: new[] { "CurrencyCode", "EffectiveAtUtc" },
                unique: true);
        }
    }
}
