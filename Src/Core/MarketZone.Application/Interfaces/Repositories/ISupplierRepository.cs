using System.Threading.Tasks;
using System.Collections.Generic;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Suppliers.DTOs;
using MarketZone.Domain.Suppliers.Entities;
using MarketZone.Application.Parameters;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface ISupplierRepository : IGenericRepository<Supplier>
	{
		Task<PaginationResponseDto<SupplierDto>> GetPagedListAsync(SupplierFilter filter);
		Task<List<SelectListDto>> GetActiveSelectListAsync();
		Task<bool> HasAnyTransactionsAsync(long supplierId);
	}
}



