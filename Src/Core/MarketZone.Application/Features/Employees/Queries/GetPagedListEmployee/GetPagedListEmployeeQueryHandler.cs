using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Employees.DTOs;

namespace MarketZone.Application.Features.Employees.Queries.GetPagedListEmployee
{
	public class GetPagedListEmployeeQueryHandler(IEmployeeRepository employeeRepository) : IRequestHandler<GetPagedListEmployeeQuery, PagedResponse<EmployeeDto>>
	{
		public async Task<PagedResponse<EmployeeDto>> Handle(GetPagedListEmployeeQuery request, CancellationToken cancellationToken)
		{
			return await employeeRepository.GetPagedListAsync(request);
		}
	}
}



