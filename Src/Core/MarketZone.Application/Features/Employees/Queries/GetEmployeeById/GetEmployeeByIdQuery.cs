using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Employees.DTOs;

namespace MarketZone.Application.Features.Employees.Queries.GetEmployeeById
{
	public class GetEmployeeByIdQuery : IRequest<BaseResult<EmployeeDto>>
	{
		public long Id { get; set; }
	}
}



