using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.ExchangeTransactions.Queries.GetPagedListExchangeTransaction
{
    public class GetPagedListExchangeTransactionQueryHandler(IExchangeTransactionRepository repository) : IRequestHandler<GetPagedListExchangeTransactionQuery, PagedResponse<ExchangeTransactionDto>>
    {
        public async Task<PagedResponse<ExchangeTransactionDto>> Handle(GetPagedListExchangeTransactionQuery request, CancellationToken cancellationToken)
        {
            return await repository.GetPagedListAsync(request);
        }
    }
}


