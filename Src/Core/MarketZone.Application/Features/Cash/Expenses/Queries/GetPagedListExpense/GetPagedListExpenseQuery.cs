using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Enums;
using System;

namespace MarketZone.Application.Features.Cash.Expenses.Queries.GetPagedListExpense
{
    public class GetPagedListExpenseQuery : IRequest<PagedResponse<CashTransactionDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public long? CashRegisterId { get; set; }
        public TransactionType? TransactionType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public ReferenceType? ReferenceType { get; set; }
        public long? ReferenceId { get; set; }
    }
}
