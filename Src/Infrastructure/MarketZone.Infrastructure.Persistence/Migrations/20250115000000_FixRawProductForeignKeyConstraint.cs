using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketZone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixRawProductForeignKeyConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // التحقق من وجود العمود أولاً
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                              WHERE TABLE_NAME = 'RoastingInvoiceDetails' 
                              AND COLUMN_NAME = 'RawProductId')
                BEGIN
                    -- إضافة العمود إذا لم يكن موجوداً
                    ALTER TABLE RoastingInvoiceDetails 
                    ADD RawProductId bigint NULL;
                END
            ");

            // حذف Foreign Key constraint الموجود (إذا كان موجوداً)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RoastingInvoiceDetails_Products_RawProductId')
                BEGIN
                    ALTER TABLE RoastingInvoiceDetails DROP CONSTRAINT FK_RoastingInvoiceDetails_Products_RawProductId;
                END
            ");

            // تغيير نوع البيانات إلى nullable (إذا كان العمود موجوداً)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                          WHERE TABLE_NAME = 'RoastingInvoiceDetails' 
                          AND COLUMN_NAME = 'RawProductId'
                          AND IS_NULLABLE = 'NO')
                BEGIN
                    ALTER TABLE RoastingInvoiceDetails 
                    ALTER COLUMN RawProductId bigint NULL;
                END
            ");

            // إعادة إنشاء Foreign Key constraint مع SetNull
            migrationBuilder.AddForeignKey(
                name: "FK_RoastingInvoiceDetails_Products_RawProductId",
                table: "RoastingInvoiceDetails",
                column: "RawProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // حذف Foreign Key constraint الجديد (إذا كان موجوداً)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RoastingInvoiceDetails_Products_RawProductId')
                BEGIN
                    ALTER TABLE RoastingInvoiceDetails DROP CONSTRAINT FK_RoastingInvoiceDetails_Products_RawProductId;
                END
            ");

            // إعادة تغيير نوع البيانات إلى non-nullable (إذا كان العمود موجوداً)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                          WHERE TABLE_NAME = 'RoastingInvoiceDetails' 
                          AND COLUMN_NAME = 'RawProductId')
                BEGIN
                    ALTER TABLE RoastingInvoiceDetails 
                    ALTER COLUMN RawProductId bigint NOT NULL;
                END
            ");

            // إعادة إنشاء Foreign Key constraint الأصلي مع Restrict
            migrationBuilder.AddForeignKey(
                name: "FK_RoastingInvoiceDetails_Products_RawProductId",
                table: "RoastingInvoiceDetails",
                column: "RawProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
