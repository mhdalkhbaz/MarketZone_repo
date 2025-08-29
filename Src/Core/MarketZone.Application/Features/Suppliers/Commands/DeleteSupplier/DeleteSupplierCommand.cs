using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Suppliers.Commands.DeleteSupplier
{
	public class DeleteSupplierCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
	}
}



