using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Suppliers.DTOs;

namespace MarketZone.Application.Features.Suppliers.Queries.GetPagedListSupplier
{
	public class GetPagedListSupplierQueryHandler(ISupplierRepository supplierRepository) : IRequestHandler<GetPagedListSupplierQuery, PagedResponse<SupplierDto>>
	{
		public async Task<PagedResponse<SupplierDto>> Handle(GetPagedListSupplierQuery request, CancellationToken cancellationToken)
		{
			return await supplierRepository.GetPagedListAsync(request.PageNumber, request.PageSize, request.Name);
		}
	}
}



