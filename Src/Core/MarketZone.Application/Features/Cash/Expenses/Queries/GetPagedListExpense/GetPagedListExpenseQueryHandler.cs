using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.Expenses.Queries.GetPagedListExpense
{
	public class GetPagedListExpenseQueryHandler(IExpenseRepository expenseRepository) : IRequestHandler<GetPagedListExpenseQuery, PagedResponse<ExpenseDto>>
	{
		public async Task<PagedResponse<ExpenseDto>> Handle(GetPagedListExpenseQuery request, CancellationToken cancellationToken)
		{
			return await expenseRepository.GetPagedListAsync(request.PageNumber, request.PageSize, request.Description);
		}
	}
}





