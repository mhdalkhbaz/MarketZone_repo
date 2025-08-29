using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Purchases.Commands.DeletePurchaseInvoice
{
	public class DeletePurchaseInvoiceCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
	}
}



