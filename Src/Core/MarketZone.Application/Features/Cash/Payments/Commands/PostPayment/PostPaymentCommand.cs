using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Cash.Payments.Commands.PostPayment
{
	public class PostPaymentCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
	}
}


