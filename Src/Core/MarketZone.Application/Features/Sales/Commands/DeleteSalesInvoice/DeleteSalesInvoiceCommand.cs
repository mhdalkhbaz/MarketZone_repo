using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Sales.Commands.DeleteSalesInvoice
{
	public class DeleteSalesInvoiceCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
	}
}



