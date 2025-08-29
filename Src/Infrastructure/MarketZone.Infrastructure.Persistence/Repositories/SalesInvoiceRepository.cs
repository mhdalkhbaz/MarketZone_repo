using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Sales.DTOs;
using MarketZone.Domain.Sales.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class SalesInvoiceRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<SalesInvoice>(dbContext), ISalesInvoiceRepository
	{
		public async Task<PaginationResponseDto<SalesInvoiceDto>> GetPagedListAsync(int pageNumber, int pageSize, string invoiceNumber)
		{
			var query = dbContext.Set<SalesInvoice>()
				.Include(x => x.Details)
				.OrderByDescending(p => p.InvoiceDate)
				.AsQueryable();

			if (!string.IsNullOrEmpty(invoiceNumber))
			{
				query = query.Where(p => p.InvoiceNumber.Contains(invoiceNumber));
			}

			return await Paged(
				query.ProjectTo<SalesInvoiceDto>(mapper.ConfigurationProvider),
				pageNumber,
				pageSize);
		}

		public async Task<SalesInvoice> GetWithDetailsByIdAsync(long id, System.Threading.CancellationToken cancellationToken = default)
		{
			return await dbContext.Set<SalesInvoice>()
				.Include(x => x.Details)
				.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		}
	}
}



