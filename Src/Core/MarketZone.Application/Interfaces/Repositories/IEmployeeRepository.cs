using System.Threading.Tasks;
using System.Collections.Generic;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Employees.DTOs;
using MarketZone.Domain.Employees.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface IEmployeeRepository : IGenericRepository<Employee>
	{
		Task<PaginationResponseDto<EmployeeDto>> GetPagedListAsync(int pageNumber, int pageSize, string name);
		Task<List<SelectListDto>> GetActiveSelectListAsync();
	}
}



