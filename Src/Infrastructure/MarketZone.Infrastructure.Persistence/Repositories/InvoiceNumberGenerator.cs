using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces.Services;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Purchases.Entities;
using MarketZone.Domain.Roasting.Entities;
using MarketZone.Domain.Sales.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class InvoiceNumberGenerator(ApplicationDbContext dbContext) : IInvoiceNumberGenerator
	{
		// Centralized syntax and length
		private const string Separator = "-";
		private const string YearFormat = "yyyy";
		private const int SequenceLength = 5;
		private static string GetPrefix(InvoiceType type) => type switch
		{
			InvoiceType.SalesInvoice => "SI",
			InvoiceType.PurchaseInvoice => "PI",
			InvoiceType.RoastingInvoice => "RI",
			_ => "INV"
		};

		public async Task<string> GenerateAsync(InvoiceType type, CancellationToken cancellationToken = default)
		{
			var year = DateTime.UtcNow.ToString(YearFormat);
			var prefix = $"{GetPrefix(type)}{Separator}{year}{Separator}";
			string lastForYear = type switch
			{
				InvoiceType.SalesInvoice => await dbContext.Set<SalesInvoice>()
					.Where(p => p.InvoiceNumber.StartsWith(prefix))
					.OrderByDescending(p => p.InvoiceNumber)
					.Select(p => p.InvoiceNumber)
					.FirstOrDefaultAsync(cancellationToken),
				InvoiceType.PurchaseInvoice => await dbContext.Set<PurchaseInvoice>()
					.Where(p => p.InvoiceNumber.StartsWith(prefix))
					.OrderByDescending(p => p.InvoiceNumber)
					.Select(p => p.InvoiceNumber)
					.FirstOrDefaultAsync(cancellationToken),
				InvoiceType.RoastingInvoice => await dbContext.Set<RoastingInvoice>()
					.Where(p => p.InvoiceNumber.StartsWith(prefix))
					.OrderByDescending(p => p.InvoiceNumber)
					.Select(p => p.InvoiceNumber)
					.FirstOrDefaultAsync(cancellationToken),
				_ => null!
			};

			int nextSeq = 1;
			if (!string.IsNullOrEmpty(lastForYear))
			{
				var parts = lastForYear.Split(Separator);
				if (parts.Length == 3 && int.TryParse(parts[2], out var seq))
				{
					nextSeq = seq + 1;
				}
			}

			return $"{prefix}{nextSeq.ToString($"D{SequenceLength}")}";
		}
	}
}




