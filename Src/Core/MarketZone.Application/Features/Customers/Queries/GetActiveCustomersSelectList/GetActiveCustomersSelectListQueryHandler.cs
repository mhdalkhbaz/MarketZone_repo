using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Customers.Queries.GetActiveCustomersSelectList
{
	public class GetActiveCustomersSelectListQueryHandler(ICustomerRepository customerRepository) : IRequestHandler<GetActiveCustomersSelectListQuery, BaseResult<List<SelectListDto>>>
	{
		public async Task<BaseResult<List<SelectListDto>>> Handle(GetActiveCustomersSelectListQuery request, CancellationToken cancellationToken)
		{
			var list = await customerRepository.GetActiveSelectListAsync();
			return list;
		}
	}
}



