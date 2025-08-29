using System.Collections.Generic;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Employees.Queries.GetActiveEmployeesSelectList
{
	public class GetActiveEmployeesSelectListQuery : IRequest<BaseResult<List<SelectListDto>>>
	{
	}
}



