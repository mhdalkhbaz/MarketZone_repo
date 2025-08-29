using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Customers.Commands.UpdateCustomer
{
	public class UpdateCustomerCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string WhatsAppPhone { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public bool? IsActive { get; set; }
	}
}



