using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketZone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class editproducbalance2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductBalances_ProductId_IsRoasted",
                table: "ProductBalances");

            migrationBuilder.DropColumn(
                name: "IsRoasted",
                table: "ProductBalances");

            migrationBuilder.CreateTable(
                name: "UnroastedInventory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnroastedInventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnroastedInventory_Products_ProductId",
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
                    Qty = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "IX_ProductBalances_ProductId",
                table: "ProductBalances",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UnroastedInventory_ProductId",
                table: "UnroastedInventory",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnroastedProdcutBalances_ProductId",
                table: "UnroastedProdcutBalances",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnroastedInventory");

            migrationBuilder.DropTable(
                name: "UnroastedProdcutBalances");

            migrationBuilder.DropIndex(
                name: "IX_ProductBalances_ProductId",
                table: "ProductBalances");

            migrationBuilder.AddColumn<bool>(
                name: "IsRoasted",
                table: "ProductBalances",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductBalances_ProductId_IsRoasted",
                table: "ProductBalances",
                columns: new[] { "ProductId", "IsRoasted" },
                unique: true,
                filter: "[IsRoasted] IS NOT NULL");
        }
    }
}
