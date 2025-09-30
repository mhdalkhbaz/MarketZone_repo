using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Purchases.DTOs;
using MarketZone.Domain.Purchases.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class PurchaseInvoiceRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<PurchaseInvoice>(dbContext), IPurchaseInvoiceRepository
	{
		public async Task<PaginationResponseDto<PurchaseInvoiceDto>> GetPagedListAsync(int pageNumber, int pageSize, string invoiceNumber)
		{
			var query = dbContext.Set<PurchaseInvoice>()
				.Include(x => x.Details)
				.OrderByDescending(p => p.InvoiceDate)
				.AsQueryable();

			if (!string.IsNullOrEmpty(invoiceNumber))
			{
				query = query.Where(p => p.InvoiceNumber.Contains(invoiceNumber));
			}

			return await Paged(
				query.ProjectTo<PurchaseInvoiceDto>(mapper.ConfigurationProvider),
				pageNumber,
				pageSize);
		}

		public async Task<string> GetNextInvoiceNumberAsync(CancellationToken cancellationToken = default)
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

		public async Task<PurchaseInvoice> GetWithDetailsByIdAsync(long id, CancellationToken cancellationToken = default)
		{
			return await dbContext.Set<PurchaseInvoice>()
				.Include(x => x.Details)
				.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		}
	}
}



