using System;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.ExchangeTransactions.Queries.GetPagedListExchangeTransaction
{
    public class GetPagedListExchangeTransactionQuery : IRequest<PagedResponse<ExchangeTransactionDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public long? CashRegisterId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}


