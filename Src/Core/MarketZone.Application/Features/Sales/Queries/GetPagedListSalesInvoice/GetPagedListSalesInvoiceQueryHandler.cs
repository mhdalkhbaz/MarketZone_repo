using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.DTOs;

namespace MarketZone.Application.Features.Sales.Queries.GetPagedListSalesInvoice
{
	public class GetPagedListSalesInvoiceQueryHandler(ISalesInvoiceRepository repository) : IRequestHandler<GetPagedListSalesInvoiceQuery, PagedResponse<SalesInvoiceDto>>
	{
		public async Task<PagedResponse<SalesInvoiceDto>> Handle(GetPagedListSalesInvoiceQuery request, CancellationToken cancellationToken)
		{
			return await repository.GetPagedListAsync(request.PageNumber, request.PageSize, request.InvoiceNumber);
		}
	}
}



