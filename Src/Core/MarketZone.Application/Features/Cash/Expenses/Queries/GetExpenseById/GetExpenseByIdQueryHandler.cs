using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Application.Helpers;

namespace MarketZone.Application.Features.Cash.Expenses.Queries.GetExpenseById
{
	public class GetExpenseByIdQueryHandler(ICashTransactionRepository repository, ITranslator translator) : IRequestHandler<GetExpenseByIdQuery, BaseResult<CashTransactionDto>>
	{
		public async Task<BaseResult<CashTransactionDto>> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
			if (entity == null)
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ExpenseMessages.Expense_NotFound_with_id(request.Id)), nameof(request.Id));

            var dto = new CashTransactionDto(entity);
            return dto;
        }
    }
}
