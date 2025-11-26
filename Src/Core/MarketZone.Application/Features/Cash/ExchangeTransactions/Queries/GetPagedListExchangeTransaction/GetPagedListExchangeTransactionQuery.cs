using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.ExchangeTransactions.Queries.GetPagedListExchangeTransaction
{
    public class GetPagedListExchangeTransactionQuery : ExchangeTransactionFilter, IRequest<PagedResponse<ExchangeTransactionDto>>
    {
    }
}


