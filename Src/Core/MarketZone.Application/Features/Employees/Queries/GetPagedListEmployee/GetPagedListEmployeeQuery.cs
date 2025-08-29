using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Employees.DTOs;

namespace MarketZone.Application.Features.Employees.Queries.GetPagedListEmployee
{
	public class GetPagedListEmployeeQuery : PaginationRequestParameter, IRequest<PagedResponse<EmployeeDto>>
	{
		public string Name { get; set; }
	}
}



