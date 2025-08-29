using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces.Services;
using MarketZone.Domain.Purchases.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class PurchaseInvoiceNumberGenerator(ApplicationDbContext dbContext) : IPurchaseInvoiceNumberGenerator
	{
		public async Task<string> GenerateAsync(CancellationToken cancellationToken = default)
		{
			var year = DateTime.UtcNow.Year;
			var prefix = $"SM-{year}-";
			var lastForYear = await dbContext.Set<PurchaseInvoice>()
				.Where(p => p.InvoiceNumber.StartsWith(prefix))
				.OrderByDescending(p => p.InvoiceNumber)
				.Select(p => p.InvoiceNumber)
				.FirstOrDefaultAsync(cancellationToken);

			int nextSeq = 1;
			if (!string.IsNullOrEmpty(lastForYear))
			{
				var parts = lastForYear.Split('-');
				if (parts.Length == 3 && int.TryParse(parts[2], out var seq))
				{
					nextSeq = seq + 1;
				}
			}

			return $"{prefix}{nextSeq.ToString("D5")}";
		}
	}
}


