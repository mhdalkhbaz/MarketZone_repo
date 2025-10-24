using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.Expenses.Queries.GetExpenseById
{
    public class GetExpenseByIdQueryHandler(ICashTransactionRepository repository) : IRequestHandler<GetExpenseByIdQuery, BaseResult<CashTransactionDto>>
    {
        public async Task<BaseResult<CashTransactionDto>> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
                return new Error(ErrorCode.NotFound, "Expense not found", nameof(request.Id));

            var dto = new CashTransactionDto(entity);
            return dto;
        }
    }
}
