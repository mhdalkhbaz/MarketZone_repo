using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Employees.Queries.GetActiveEmployeesSelectList
{
	public class GetActiveEmployeesSelectListQueryHandler(IEmployeeRepository employeeRepository) : IRequestHandler<GetActiveEmployeesSelectListQuery, BaseResult<List<SelectListDto>>>
	{
		public async Task<BaseResult<List<SelectListDto>>> Handle(GetActiveEmployeesSelectListQuery request, CancellationToken cancellationToken)
		{
			var list = await employeeRepository.GetActiveSelectListAsync();
			return list;
		}
	}
}



