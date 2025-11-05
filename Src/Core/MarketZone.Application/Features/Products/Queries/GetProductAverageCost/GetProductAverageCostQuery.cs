using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Products.Queries.GetProductAverageCost
{
	public class GetProductAverageCostQuery : IRequest<BaseResult<decimal>>
	{
		public long Id { get; set; }
	}
}





