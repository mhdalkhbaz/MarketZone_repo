using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Suppliers.DTOs;

namespace MarketZone.Application.Features.Suppliers.Queries.GetPagedListSupplier
{
	public class GetPagedListSupplierQuery : PaginationRequestParameter, IRequest<PagedResponse<SupplierDto>>
	{
		public string Name { get; set; }
	}
}



