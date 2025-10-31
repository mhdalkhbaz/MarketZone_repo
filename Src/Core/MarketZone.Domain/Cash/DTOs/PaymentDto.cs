using System;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Domain.Cash.DTOs
{
	public class PaymentDto
	{
		public PaymentDto()
		{
		}

		public PaymentDto(Payment e)
		{
			Id = e.Id;
			CashRegisterId = e.CashRegisterId;
			PaymentType = e.PaymentType;
			InvoiceId = e.InvoiceId;
			InvoiceType = e.InvoiceType;
			PaymentDate = e.PaymentDate;
			Amount = e.Amount;
			Currency = e.Currency;
			PaymentCurrency = e.PaymentCurrency;
			ExchangeRate = e.ExchangeRate;
			AmountInPaymentCurrency = e.AmountInPaymentCurrency;
			Notes = e.Notes;
			Description = e.Description;
			ReceivedBy = e.ReceivedBy;
			PaidBy = e.PaidBy;
			IsConfirmed = e.IsConfirmed;
			Status = e.Status;
			IsIncome = e.IsIncome;
			IsExpense = e.IsExpense;
		}

		public long Id { get; set; }
		public long? CashRegisterId { get; set; }
		public PaymentType PaymentType { get; set; }
		public long? InvoiceId { get; set; }
		public InvoiceType? InvoiceType { get; set; }
		public DateTime PaymentDate { get; set; }
		public decimal Amount { get; set; }
		public Currency Currency { get; set; }                    // عملة المورد/الفاتورة
		public Currency PaymentCurrency { get; set; }            // العملة المراد الدفع بها
		public decimal? ExchangeRate { get; set; }            // سعر الصرف
		public decimal? AmountInPaymentCurrency { get; set; }  // المبلغ بالعملة المدفوعة
		public string Notes { get; set; }
		public string Description { get; set; }
		public string ReceivedBy { get; set; }
		public string PaidBy { get; set; }
		public bool IsConfirmed { get; set; }
		public PaymentStatus Status { get; set; }
		public bool IsIncome { get; set; }
		public bool IsExpense { get; set; }
	}
}


