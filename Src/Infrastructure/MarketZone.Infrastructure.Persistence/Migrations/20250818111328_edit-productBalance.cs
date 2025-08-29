using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketZone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class editproductBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductBalances_ProductId",
                table: "ProductBalances");

            migrationBuilder.DropColumn(
                name: "QtyRoasted",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductBalances_ProductId_IsRoasted",
                table: "ProductBalances");

            migrationBuilder.DropColumn(
                name: "IsRoasted",
                table: "ProductBalances");

            migrationBuilder.AddColumn<decimal>(
                name: "QtyRoasted",
                table: "ProductBalances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_ProductBalances_ProductId",
                table: "ProductBalances",
                column: "ProductId",
                unique: true);
        }
    }
}
