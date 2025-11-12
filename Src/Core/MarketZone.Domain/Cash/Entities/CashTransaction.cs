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

		public CashTransaction(long cashRegisterId, TransactionType transactionType, decimal amount, Currency currency, DateTime? transactionDate, ReferenceType? referenceType, long? referenceId, string description)
		{
			CashRegisterId = cashRegisterId;
			TransactionType = transactionType;
			Amount = amount;
			Currency = currency;
			TransactionDate = transactionDate ?? DateTime.UtcNow;
			ReferenceType = referenceType;
			ReferenceId = referenceId;
			Description = description;
		}

		public long CashRegisterId { get; private set; }
		public TransactionType TransactionType { get; private set; }
		public decimal Amount { get; private set; }
		public Currency Currency { get; private set; }
		public DateTime TransactionDate { get; private set; }
		public ReferenceType? ReferenceType { get; private set; }
		public long? ReferenceId { get; private set; }
		public string Description { get; private set; }

		public void Update(long? cashRegisterId, TransactionType? transactionType, decimal? amount, Currency? currency, DateTime? transactionDate, ReferenceType? referenceType, long? referenceId, string description)
		{
			if (cashRegisterId.HasValue)
				CashRegisterId = cashRegisterId.Value;
			if (transactionType.HasValue)
				TransactionType = transactionType.Value;
			if (amount.HasValue)
				Amount = amount.Value;
			if (currency.HasValue)
				Currency = currency.Value;
			if (transactionDate.HasValue)
				TransactionDate = transactionDate.Value;
			if (referenceType.HasValue)
				ReferenceType = referenceType.Value;
			if (referenceId.HasValue)
				ReferenceId = referenceId.Value;
			if (!string.IsNullOrEmpty(description))
				Description = description;
		}
	}
}


