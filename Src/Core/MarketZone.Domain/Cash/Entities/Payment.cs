using System;
using MarketZone.Domain.Common;

namespace MarketZone.Domain.Cash.Entities
{
	public class Payment : AuditableBaseEntity
	{
		private Payment()
		{
		}

		public Payment(long invoiceId, decimal amount, DateTime? paymentDate, string notes, string receivedBy, bool isConfirmed = true)
		{
			InvoiceId = invoiceId;
			Amount = amount;
			PaymentDate = paymentDate ?? DateTime.UtcNow;
			Notes = notes;
			ReceivedBy = receivedBy;
			IsConfirmed = isConfirmed;
			Status = PaymentStatus.Draft;
		}

		public long? CashRegisterId { get; private set; }
		public long InvoiceId { get; private set; }
		public DateTime PaymentDate { get; private set; }
		public decimal Amount { get; private set; }
		public string Notes { get; private set; }
		public string ReceivedBy { get; private set; }
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


