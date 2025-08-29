using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.DTOs;

namespace MarketZone.Application.Features.Sales.Queries.GetSalesInvoiceById
{
	public class GetSalesInvoiceByIdQuery : IRequest<BaseResult<SalesInvoiceDto>>
	{
		public long Id { get; set; }
	}
}



