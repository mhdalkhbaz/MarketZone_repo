using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.Expenses.Queries.GetExpenseById
{
    public class GetExpenseByIdQuery : IRequest<BaseResult<CashTransactionDto>>
    {
        public long Id { get; set; }
    }
}
