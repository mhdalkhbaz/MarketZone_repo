using System;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash.Expenses.Commands.UpdateExpense
{
	public class UpdateExpenseCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
		public decimal? Amount { get; set; }
		public DateTime? ExpenseDate { get; set; }
		public string Description { get; set; }
		public ExpenseStatus? Status { get; set; }
	}
}


