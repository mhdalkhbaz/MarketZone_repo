using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
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
		public async Task<PaginationResponseDto<SupplierDto>> GetPagedListAsync(int pageNumber, int pageSize, string name)
		{
			var query = dbContext.Set<Supplier>().OrderByDescending(p => p.Created).AsQueryable();

			if (!string.IsNullOrEmpty(name))
			{
				query = query.Where(p => p.Name.Contains(name));
			}

			return await Paged(
				query.ProjectTo<SupplierDto>(mapper.ConfigurationProvider),
				pageNumber,
				pageSize);
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



