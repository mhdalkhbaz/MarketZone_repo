using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketZone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeIdToRoastingInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EmployeeId",
                table: "RoastingInvoices",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoastingInvoices_EmployeeId",
                table: "RoastingInvoices",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoastingInvoices_Employees_EmployeeId",
                table: "RoastingInvoices",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoastingInvoices_Employees_EmployeeId",
                table: "RoastingInvoices");

            migrationBuilder.DropIndex(
                name: "IX_RoastingInvoices_EmployeeId",
                table: "RoastingInvoices");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "RoastingInvoices");
        }
    }
}
