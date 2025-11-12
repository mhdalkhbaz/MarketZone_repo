using MarketZone.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
    public class EmployeeSalaryConfiguration : IEntityTypeConfiguration<EmployeeSalary>
    {
        public void Configure(EntityTypeBuilder<EmployeeSalary> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(p => p.EmployeeId).IsRequired();
            builder.Property(p => p.Year).IsRequired();
            builder.Property(p => p.Month).IsRequired();
            builder.Property(p => p.BaseSalary).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.PercentageAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(p => p.PaidAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(p => p.TotalSalary).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.RemainingAmount).HasColumnType("decimal(18,2)").IsRequired();

            // Foreign key relationship
            builder.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint: EmployeeId + Year + Month
            builder.HasIndex(x => new { x.EmployeeId, x.Year, x.Month }).IsUnique();
        }
    }
}
