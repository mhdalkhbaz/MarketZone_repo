using System;
using MarketZone.Domain.Common;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Domain.Cash.Entities
{
	public class Payment : AuditableBaseEntity
	{
		private Payment()
		{
		}

		// Constructor for invoice payments
		public Payment(long invoiceId, decimal amount, DateTime? paymentDate, string notes, string receivedBy, bool isConfirmed = true)
		{
			PaymentType = PaymentType.InvoicePayment;
			InvoiceId = invoiceId;
			Amount = amount;
			PaymentDate = paymentDate ?? DateTime.UtcNow;
			Notes = notes;
			ReceivedBy = receivedBy;
			IsConfirmed = isConfirmed;
			Status = PaymentStatus.Draft;
		}

		// Constructor for expenses
		public Payment(decimal amount, DateTime paymentDate, string description, string paidBy, bool isConfirmed = true)
		{
			PaymentType = PaymentType.Expense;
			Amount = amount;
			PaymentDate = paymentDate;
			Description = description;
			PaidBy = paidBy;
			IsConfirmed = isConfirmed;
			Status = PaymentStatus.Draft;
		}

		public long? CashRegisterId { get; private set; }
		public PaymentType PaymentType { get; private set; }
		public long? InvoiceId { get; private set; }
		public DateTime PaymentDate { get; private set; }
		public decimal Amount { get; private set; }
		public string Notes { get; private set; }
		public string Description { get; private set; }
		public string ReceivedBy { get; private set; }
		public string PaidBy { get; private set; }
		public bool IsConfirmed { get; private set; } = true;
		public PaymentStatus Status { get; private set; }

		public void AssignRegister(long? cashRegisterId) => CashRegisterId = cashRegisterId;
		public void Post() => Status = PaymentStatus.Posted;
	}

	public enum PaymentStatus : short
	{
		Draft = 0,
		Posted = 1
	}
}


