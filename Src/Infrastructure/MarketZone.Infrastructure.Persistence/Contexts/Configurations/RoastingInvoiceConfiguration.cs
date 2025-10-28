using MarketZone.Domain.Roasting.Entities;
using MarketZone.Domain.Roasting.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
    public class RoastingInvoiceConfiguration : IEntityTypeConfiguration<RoastingInvoice>
    {
        public void Configure(EntityTypeBuilder<RoastingInvoice> builder)
        {
            builder.Property(x => x.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.InvoiceDate)
                .IsRequired();

            builder.Property(x => x.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasMaxLength(500);

            builder.Property(x => x.EmployeeId)
                .IsRequired(false);

            builder.Property(x => x.Status)
                .HasConversion<short>()
                .HasDefaultValue(RoastingInvoiceStatus.Draft)
                .IsRequired();

            builder.Property(x => x.PaymentStatus)
                .HasConversion<short>()
                .HasDefaultValue(RoastingPaymentStatus.InProgress)
                .IsRequired();

            // Foreign key relationship to Employee
            builder.HasOne<MarketZone.Domain.Employees.Entities.Employee>()
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Unique index on InvoiceNumber
            builder.HasIndex(x => x.InvoiceNumber)
                .IsUnique();
        }
    }
}
