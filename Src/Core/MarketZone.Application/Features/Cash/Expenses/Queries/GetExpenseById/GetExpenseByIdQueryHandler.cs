using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.Expenses.Queries.GetExpenseById
{
	public class GetExpenseByIdQueryHandler(IExpenseRepository expenseRepository, ITranslator translator, IMapper mapper) : IRequestHandler<GetExpenseByIdQuery, BaseResult<ExpenseDto>>
	{
		public async Task<BaseResult<ExpenseDto>> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
		{
			var expense = await expenseRepository.GetByIdAsync(request.Id);

			if (expense is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ExpenseMessages.Expense_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			return mapper.Map<ExpenseDto>(expense);
		}
	}
}





