using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Purchases.DTOs;

namespace MarketZone.Application.Features.Purchases.Queries.GetPagedListPurchaseInvoice
{
	public class GetPagedListPurchaseInvoiceQuery : PurchaseInvoiceFilter, IRequest<PagedResponse<PurchaseInvoiceDto>>
	{
	}
}



