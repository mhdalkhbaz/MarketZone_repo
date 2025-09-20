using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketZone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class productedit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoastingOperations");

            migrationBuilder.DropTable(
                name: "UnroastedProdcutBalances");

            migrationBuilder.DropColumn(
                name: "BarCode2",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BarCode2",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RoastingOperations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    QuantityKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RoastDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    RoastPricePerKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoastingOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoastingOperations_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnroastedProdcutBalances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    AvailableQty = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    TotalValue = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnroastedProdcutBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnroastedProdcutBalances_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoastingOperations_ProductId",
                table: "RoastingOperations",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UnroastedProdcutBalances_ProductId",
                table: "UnroastedProdcutBalances",
                column: "ProductId",
                unique: true);
        }
    }
}
