using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Sales.Commands.PostSalesInvoice
{
	public class PostSalesInvoiceCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
	}
}


