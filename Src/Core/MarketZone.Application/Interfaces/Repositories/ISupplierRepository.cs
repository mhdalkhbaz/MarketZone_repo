using System.Threading.Tasks;
using System.Collections.Generic;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Suppliers.DTOs;
using MarketZone.Domain.Suppliers.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface ISupplierRepository : IGenericRepository<Supplier>
	{
		Task<PaginationResponseDto<SupplierDto>> GetPagedListAsync(int pageNumber, int pageSize, string name);
		Task<List<SelectListDto>> GetActiveSelectListAsync();
		Task<bool> HasAnyTransactionsAsync(long supplierId);
	}
}



