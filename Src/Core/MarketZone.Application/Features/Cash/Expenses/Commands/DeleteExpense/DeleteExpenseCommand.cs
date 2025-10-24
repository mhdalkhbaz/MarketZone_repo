using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Cash.Expenses.Commands.DeleteExpense
{
    public class DeleteExpenseCommand : IRequest<BaseResult>
    {
        public long Id { get; set; }
    }
}
