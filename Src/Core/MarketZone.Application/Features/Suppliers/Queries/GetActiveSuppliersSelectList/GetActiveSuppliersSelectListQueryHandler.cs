using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Suppliers.Queries.GetActiveSuppliersSelectList
{
	public class GetActiveSuppliersSelectListQueryHandler(ISupplierRepository supplierRepository) : IRequestHandler<GetActiveSuppliersSelectListQuery, BaseResult<List<SelectListDto>>>
	{
		public async Task<BaseResult<List<SelectListDto>>> Handle(GetActiveSuppliersSelectListQuery request, CancellationToken cancellationToken)
		{
			var list = await supplierRepository.GetActiveSelectListAsync();
			return list;
		}
	}
}



