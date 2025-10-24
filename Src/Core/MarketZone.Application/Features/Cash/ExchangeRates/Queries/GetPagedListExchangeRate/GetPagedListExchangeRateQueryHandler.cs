using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.ExchangeRates.Queries.GetPagedListExchangeRate
{
    public class GetPagedListExchangeRateQueryHandler(IExchangeRateRepository repository) : IRequestHandler<GetPagedListExchangeRateQuery, PagedResponse<ExchangeRateDto>>
    {
        public async Task<PagedResponse<ExchangeRateDto>> Handle(GetPagedListExchangeRateQuery request, CancellationToken cancellationToken)
        {
            var paged = await repository.GetPagedListAsync(request.PageNumber, request.PageSize, request.IsActive);
            return paged;
        }
    }
}
