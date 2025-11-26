using System.Threading.Tasks;
using System.Collections.Generic;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Customers.DTOs;
using MarketZone.Domain.Customers.Entities;
using MarketZone.Application.Parameters;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface ICustomerRepository : IGenericRepository<Customer>
	{
		Task<PaginationResponseDto<CustomerDto>> GetPagedListAsync(CustomerFilter filter);
		Task<List<SelectListDto>> GetActiveSelectListAsync();
	}
}



