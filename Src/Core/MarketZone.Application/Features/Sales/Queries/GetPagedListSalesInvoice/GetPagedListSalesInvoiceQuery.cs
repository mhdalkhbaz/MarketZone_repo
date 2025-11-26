using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.DTOs;

namespace MarketZone.Application.Features.Sales.Queries.GetPagedListSalesInvoice
{
	public class GetPagedListSalesInvoiceQuery : SalesInvoiceFilter, IRequest<PagedResponse<SalesInvoiceDto>>
	{
	}
}



