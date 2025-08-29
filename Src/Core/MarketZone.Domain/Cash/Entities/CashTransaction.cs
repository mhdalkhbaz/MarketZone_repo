using System;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Common;

namespace MarketZone.Domain.Cash.Entities
{
	public class CashTransaction : AuditableBaseEntity
	{
		private CashTransaction()
		{
		}

		public CashTransaction(long cashRegisterId, TransactionType transactionType, decimal amount, DateTime? transactionDate, ReferenceType? referenceType, long? referenceId, string description)
		{
			CashRegisterId = cashRegisterId;
			TransactionType = transactionType;
			Amount = amount;
			TransactionDate = transactionDate ?? DateTime.UtcNow;
			ReferenceType = referenceType;
			ReferenceId = referenceId;
			Description = description;
		}

		public long CashRegisterId { get; private set; }
		public TransactionType TransactionType { get; private set; }
		public decimal Amount { get; private set; }
		public DateTime TransactionDate { get; private set; }
		public ReferenceType? ReferenceType { get; private set; }
		public long? ReferenceId { get; private set; }
		public string Description { get; private set; }
	}
}


