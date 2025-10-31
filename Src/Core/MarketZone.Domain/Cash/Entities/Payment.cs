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

		// Constructor for invoice-related payments (Sales, Purchase, Roasting)
		public Payment(PaymentType paymentType, long invoiceId, InvoiceType invoiceType, decimal amount, DateTime? paymentDate, Currency currency, Currency paymentCurrency, decimal? exchangeRate, string notes, string receivedBy, bool isConfirmed = true)
		{
			if (!RequiresInvoice(paymentType))
				throw new ArgumentException("This payment type requires an invoice", nameof(paymentType));

			PaymentType = paymentType;
			InvoiceId = invoiceId;
			InvoiceType = invoiceType;
			Amount = amount;
			PaymentDate = paymentDate ?? DateTime.UtcNow;
			Currency = currency;
			PaymentCurrency = paymentCurrency;
			ExchangeRate = exchangeRate;
			
			// Calculate amount in payment currency
			if (currency == paymentCurrency)
			{
				AmountInPaymentCurrency = amount;
			}
			else if (exchangeRate.HasValue)
			{
				AmountInPaymentCurrency = amount * exchangeRate.Value;
			}
			
			Notes = notes;
			ReceivedBy = receivedBy;
			IsConfirmed = isConfirmed;
			Status = PaymentStatus.Draft;
		}

		// Constructor for general expenses (not invoice-related)
		public Payment(PaymentType paymentType, decimal amount, DateTime paymentDate, string description, string paidBy, bool isConfirmed = true)
		{
			if (RequiresInvoice(paymentType))
				throw new ArgumentException("This payment type requires an invoice", nameof(paymentType));

			PaymentType = paymentType;
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
		public InvoiceType? InvoiceType { get; private set; }
		public DateTime PaymentDate { get; private set; }
		public decimal Amount { get; private set; }
		public Currency Currency { get; private set; }           // عملة المورد/الفاتورة
		public Currency PaymentCurrency { get; private set; }     // العملة المراد الدفع بها
		public decimal? ExchangeRate { get; private set; }     // سعر الصرف
		public decimal? AmountInPaymentCurrency { get; private set; } // المبلغ بالعملة المدفوعة
		public string Notes { get; private set; }
		public string Description { get; private set; }
		public string ReceivedBy { get; private set; }
		public string PaidBy { get; private set; }
		public bool IsConfirmed { get; private set; } = true;
		public PaymentStatus Status { get; private set; }

		// Helper method to determine if payment type requires an invoice
		private static bool RequiresInvoice(PaymentType type)
		{
			return type == PaymentType.SalesPayment ||
				   type == PaymentType.PurchasePayment ||
				   type == PaymentType.RoastingPayment;
		}

		// Helper method to determine if payment is income
		public bool IsIncome => PaymentType == PaymentType.SalesPayment;

		// Helper method to determine if payment is expense
		public bool IsExpense => PaymentType != PaymentType.SalesPayment;

		public void AssignRegister(long? cashRegisterId) => CashRegisterId = cashRegisterId;
		public void Post() => Status = PaymentStatus.Posted;
	}

	public enum PaymentStatus : short
	{
		Draft = 0,
		Posted = 1
	}
}


