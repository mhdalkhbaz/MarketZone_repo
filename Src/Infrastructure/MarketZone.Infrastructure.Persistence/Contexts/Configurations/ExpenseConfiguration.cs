using MarketZone.Domain.Cash.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
	{
		public void Configure(EntityTypeBuilder<Expense> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(p => p.ExpenseDate).HasColumnType("date").IsRequired();
			builder.Property(p => p.Description).HasMaxLength(500);
			builder.Property(p => p.Status).HasConversion<short>().HasDefaultValue(ExpenseStatus.Draft);
		}
	}
}


