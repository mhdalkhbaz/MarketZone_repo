using System.Collections.Generic;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Suppliers.Queries.GetActiveSuppliersSelectList
{
	public class GetActiveSuppliersSelectListQuery : IRequest<BaseResult<List<SelectListDto>>>
	{
	}
}



