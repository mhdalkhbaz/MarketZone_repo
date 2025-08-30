﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketZone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class roastin22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualQuantityAfterRoasting",
                table: "RoastingInvoiceDetails",
                type: "decimal(18,6)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualQuantityAfterRoasting",
                table: "RoastingInvoiceDetails");
        }
    }
}
