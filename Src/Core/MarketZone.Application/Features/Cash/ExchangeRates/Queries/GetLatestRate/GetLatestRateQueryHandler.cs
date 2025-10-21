using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash.ExchangeRates.Queries.GetLatestRate
{
    public class GetLatestRateQueryHandler(IExchangeRateRepository repository) : IRequestHandler<GetLatestRateQuery, BaseResult<ExchangeRate>>
    {
        public async Task<BaseResult<ExchangeRate>> Handle(GetLatestRateQuery request, CancellationToken cancellationToken)
        {
            var latestRate = await repository.GetLatestActiveRateAsync(cancellationToken);
            if (latestRate == null)
                return new Error(ErrorCode.NotFound, "No active exchange rate found");

            return latestRate;
        }
    }
}