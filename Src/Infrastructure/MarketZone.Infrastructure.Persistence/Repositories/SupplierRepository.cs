using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using System.Collections.Generic;
using MarketZone.Domain.Suppliers.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Suppliers.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class SupplierRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<Supplier>(dbContext), ISupplierRepository
	{
		public async Task<PaginationResponseDto<SupplierDto>> GetPagedListAsync(SupplierFilter filter)
		{
			var query = dbContext.Set<Supplier>().AsQueryable();

			// Apply filters using FilterBuilder pattern
			if (!string.IsNullOrEmpty(filter.Name))
			{
				query = query.Where(p => p.Name.Contains(filter.Name));
			}

			if (!string.IsNullOrEmpty(filter.Address))
			{
				query = query.Where(p => !string.IsNullOrEmpty(p.Address) && p.Address.Contains(filter.Address));
			}

			if (filter.Status.HasValue)
			{
				// Map status to IsActive: 1 = true, 0 = false
				query = query.Where(p => p.IsActive == (filter.Status.Value == 1));
			}

			query = query.OrderByDescending(p => p.Created);

			return await Paged(
				query.ProjectTo<SupplierDto>(mapper.ConfigurationProvider),
				filter.PageNumber,
				filter.PageSize);
		}

		public async Task<List<SelectListDto>> GetActiveSelectListAsync()
		{
			return await dbContext.Set<Supplier>()
				.Where(x => x.IsActive)
				.OrderBy(x => x.Name)
				.Select(x => new SelectListDto(x.Name, x.Id.ToString()))
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<bool> HasAnyTransactionsAsync(long supplierId)
		{
			// Any purchase invoice linked to this supplier counts as a related transaction
			return await dbContext.PurchaseInvoices.AnyAsync(pi => pi.SupplierId == supplierId);
		}
	}
}



