using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketZone.Infrastructure.Persistence.Migrations
{
    public partial class AddAvailableQtyToUnroastedProductBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AvailableQty",
                table: "UnroastedProdcutBalances",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m);

            // Update existing records to set AvailableQty = Qty
            migrationBuilder.Sql("UPDATE UnroastedProdcutBalances SET AvailableQty = Qty WHERE AvailableQty = 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableQty",
                table: "UnroastedProdcutBalances");
        }
    }
}
