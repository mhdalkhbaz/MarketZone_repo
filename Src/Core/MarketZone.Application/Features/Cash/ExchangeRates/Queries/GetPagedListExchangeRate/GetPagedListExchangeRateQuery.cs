using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.ExchangeRates.Queries.GetPagedListExchangeRate
{
    public class GetPagedListExchangeRateQuery : ExchangeRateFilter, IRequest<PagedResponse<ExchangeRateDto>>
    {
    }
}
