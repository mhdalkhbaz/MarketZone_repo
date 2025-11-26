using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.Expenses.Queries.GetPagedListExpense
{
    public class GetPagedListExpenseQueryHandler(ICashTransactionRepository repository) : IRequestHandler<GetPagedListExpenseQuery, PagedResponse<CashTransactionDto>>
    {
        public async Task<PagedResponse<CashTransactionDto>> Handle(GetPagedListExpenseQuery request, CancellationToken cancellationToken)
        {
            return await repository.GetPagedListAsync(request);
        }
    }
}
