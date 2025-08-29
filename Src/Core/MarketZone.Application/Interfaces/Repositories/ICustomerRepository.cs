using System.Threading.Tasks;
using System.Collections.Generic;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Customers.DTOs;
using MarketZone.Domain.Customers.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface ICustomerRepository : IGenericRepository<Customer>
	{
		Task<PaginationResponseDto<CustomerDto>> GetPagedListAsync(int pageNumber, int pageSize, string name);
		Task<List<SelectListDto>> GetActiveSelectListAsync();
	}
}



