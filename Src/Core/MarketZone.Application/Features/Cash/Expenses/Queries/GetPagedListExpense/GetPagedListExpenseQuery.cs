using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.Expenses.Queries.GetPagedListExpense
{
	public class GetPagedListExpenseQuery : PaginationRequestParameter, IRequest<PagedResponse<ExpenseDto>>
	{
		public string Description { get; set; }
	}
}





