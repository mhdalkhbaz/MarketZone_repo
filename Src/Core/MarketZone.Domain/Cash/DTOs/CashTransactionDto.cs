using System;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Domain.Cash.DTOs
{
	public class CashTransactionDto
	{
		public CashTransactionDto()
		{
		}

		public CashTransactionDto(CashTransaction e)
		{
			Id = e.Id;
			CashRegisterId = e.CashRegisterId;
			TransactionType = e.TransactionType;
			Amount = e.Amount;
			Currency = e.Currency;
			TransactionDate = e.TransactionDate;
			ReferenceType = e.ReferenceType;
			ReferenceId = e.ReferenceId;
			Description = e.Description;
		}

		public long Id { get; set; }
		public long CashRegisterId { get; set; }
		public TransactionType TransactionType { get; set; }
		public decimal Amount { get; set; }
		public Currency Currency { get; set; }
		public DateTime TransactionDate { get; set; }
		public ReferenceType? ReferenceType { get; set; }
		public long? ReferenceId { get; set; }
		public string Description { get; set; }
	}
}


