using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Customers.Commands.DeleteCustomer
{
	public class DeleteCustomerCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
	}
}



