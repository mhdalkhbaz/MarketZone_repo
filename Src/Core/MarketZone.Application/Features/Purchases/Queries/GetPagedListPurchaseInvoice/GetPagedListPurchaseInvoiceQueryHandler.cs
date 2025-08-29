using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Purchases.DTOs;

namespace MarketZone.Application.Features.Purchases.Queries.GetPagedListPurchaseInvoice
{
	public class GetPagedListPurchaseInvoiceQueryHandler(IPurchaseInvoiceRepository repository) : IRequestHandler<GetPagedListPurchaseInvoiceQuery, PagedResponse<PurchaseInvoiceDto>>
	{
		public async Task<PagedResponse<PurchaseInvoiceDto>> Handle(GetPagedListPurchaseInvoiceQuery request, CancellationToken cancellationToken)
		{
			return await repository.GetPagedListAsync(request.PageNumber, request.PageSize, request.InvoiceNumber);
		}
	}
}



