using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash.ExchangeRates.Queries.GetLatestRate
{
    public class GetLatestRateQuery : IRequest<BaseResult<ExchangeRate>>
    {
        // No parameters needed - gets the latest active rate
    }
}