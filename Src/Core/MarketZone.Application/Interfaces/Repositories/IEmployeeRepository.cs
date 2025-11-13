using System.Threading.Tasks;
using System.Collections.Generic;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Employees.DTOs;
using MarketZone.Domain.Employees.Entities;
using MarketZone.Domain.Employees.Enums;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface IEmployeeRepository : IGenericRepository<Employee>
	{
		Task<PaginationResponseDto<EmployeeDto>> GetPagedListAsync(int pageNumber, int pageSize, string name);
		Task<List<SelectListDto>> GetActiveSelectListAsync(string? type = null);
		Task<Dictionary<long, Currency?>> GetEmployeeCurrenciesAsync(List<long> employeeIds, System.Threading.CancellationToken cancellationToken = default);
	}
}



