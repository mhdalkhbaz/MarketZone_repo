using System;
using MarketZone.Domain.Common;

namespace MarketZone.Domain.Cash.Entities
{
	public class Expense : AuditableBaseEntity
	{
		private Expense()
		{
		}

		public Expense(decimal amount, DateTime expenseDate, string description)
		{
			Amount = amount;
			ExpenseDate = expenseDate;
			Description = description;
			Status = ExpenseStatus.Draft;
		}

		public decimal Amount { get; private set; }
		public DateTime ExpenseDate { get; private set; }
		public string Description { get; private set; }
		public ExpenseStatus Status { get; private set; }

		public void Post() => Status = ExpenseStatus.Posted;
	}

	public enum ExpenseStatus : short
	{
		Draft = 0,
		Posted = 1
	}
}


