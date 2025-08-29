using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Suppliers.DTOs;

namespace MarketZone.Application.Features.Suppliers.Queries.GetSupplierById
{
	public class GetSupplierByIdQuery : IRequest<BaseResult<SupplierDto>>
	{
		public long Id { get; set; }
	}
}



