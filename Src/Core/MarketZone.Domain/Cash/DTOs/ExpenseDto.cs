using System;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Domain.Cash.DTOs
{
	public class ExpenseDto
	{
		public ExpenseDto()
		{
		}

		public ExpenseDto(Expense e)
		{
			Id = e.Id;
			Amount = e.Amount;
			ExpenseDate = e.ExpenseDate;
			Description = e.Description;
			Status = e.Status;
			CreatedDateTime = e.Created;
		}

		public long Id { get; set; }
		public decimal Amount { get; set; }
		public DateTime ExpenseDate { get; set; }
		public string Description { get; set; }
		public ExpenseStatus Status { get; set; }
		public DateTime CreatedDateTime { get; set; }
	}
}


