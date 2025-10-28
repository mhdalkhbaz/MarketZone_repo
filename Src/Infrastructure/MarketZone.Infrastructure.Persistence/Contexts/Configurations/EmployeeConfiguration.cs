using MarketZone.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
	{
		public void Configure(EntityTypeBuilder<Employee> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
			builder.Property(p => p.LastName).HasMaxLength(100).IsRequired();
			builder.Property(p => p.Phone).HasMaxLength(50);
			builder.Property(p => p.WhatsAppPhone).HasMaxLength(50);
			builder.Property(p => p.Email).HasMaxLength(100);
			builder.Property(p => p.Address).HasMaxLength(500);
			builder.Property(p => p.JobTitle).HasMaxLength(100);
			builder.Property(p => p.Salary).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(p => p.HireDate).HasColumnType("date").IsRequired();
			builder.Property(p => p.IsActive).HasDefaultValue(true);
			builder.Property(p => p.SyrianMoney).HasColumnType("decimal(18,2)");
			builder.Property(p => p.DollarMoney).HasColumnType("decimal(18,2)");
		}
	}
}



