using MarketZone.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
    public class SalaryPaymentConfiguration : IEntityTypeConfiguration<SalaryPayment>
    {
        public void Configure(EntityTypeBuilder<SalaryPayment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(p => p.EmployeeId).IsRequired();
            builder.Property(p => p.Year).IsRequired();
            builder.Property(p => p.Month).IsRequired();
            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.PaymentDate).HasColumnType("datetime2").IsRequired();
            builder.Property(p => p.Notes).HasMaxLength(1000);
            builder.Property(p => p.SalaryType).HasConversion<int>().IsRequired();
            builder.Property(p => p.CashRegisterId).IsRequired(false);
            builder.Property(p => p.DistributionTripId).IsRequired(false);

            // Foreign key relationships - استخدام Navigation Properties لتجنب التكرار
            builder.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.CashRegister)
                .WithMany()
                .HasForeignKey(x => x.CashRegisterId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.DistributionTrip)
                .WithMany()
                .HasForeignKey(x => x.DistributionTripId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
