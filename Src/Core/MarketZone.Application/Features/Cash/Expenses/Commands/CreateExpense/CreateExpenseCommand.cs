using System;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Cash.Expenses.Commands.CreateExpense
{
	public class CreateExpenseCommand : IRequest<BaseResult<long>>
	{
		public decimal Amount { get; set; }
		public DateTime ExpenseDate { get; set; }
		public string Description { get; set; }
	}
}


