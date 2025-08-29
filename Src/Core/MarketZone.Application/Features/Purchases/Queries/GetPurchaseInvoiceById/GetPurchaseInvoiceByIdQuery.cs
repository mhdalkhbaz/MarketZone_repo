using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Purchases.DTOs;

namespace MarketZone.Application.Features.Purchases.Queries.GetPurchaseInvoiceById
{
	public class GetPurchaseInvoiceByIdQuery : IRequest<BaseResult<PurchaseInvoiceDto>>
	{
		public long Id { get; set; }
	}
}



