using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Products.Queries.GetProductAverageCost
{
	public class GetProductAverageCostQueryHandler(IProductBalanceRepository productBalanceRepository, ITranslator translator) : IRequestHandler<GetProductAverageCostQuery, BaseResult<decimal>>
	{
		public async Task<BaseResult<decimal>> Handle(GetProductAverageCostQuery request, CancellationToken cancellationToken)
		{
			var balance = await productBalanceRepository.GetByProductIdAsync(request.Id, cancellationToken);
			var avg = balance?.AverageCost ?? 0m;
			return new BaseResult<decimal> { Success = true, Data = avg };
		}
	}
}




