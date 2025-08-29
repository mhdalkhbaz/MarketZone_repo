using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Purchases.Commands.PostPurchaseInvoice
{
	public class PostPurchaseInvoiceCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
	}
}


