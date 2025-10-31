using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Features.Customers.Commands.CreateCustomer
{
	public class CreateCustomerCommand : IRequest<BaseResult<long>>
	{
		public string Name { get; set; }
		public string Phone { get; set; }
		public string WhatsAppPhone { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public Currency? Currency { get; set; }
		public bool IsActive { get; set; } = true;
	}
}



