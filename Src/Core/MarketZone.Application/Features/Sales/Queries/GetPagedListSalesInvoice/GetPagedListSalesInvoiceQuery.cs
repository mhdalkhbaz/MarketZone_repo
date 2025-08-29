using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.DTOs;

namespace MarketZone.Application.Features.Sales.Queries.GetPagedListSalesInvoice
{
	public class GetPagedListSalesInvoiceQuery : PaginationRequestParameter, IRequest<PagedResponse<SalesInvoiceDto>>
	{
		public string InvoiceNumber { get; set; }
	}
}



