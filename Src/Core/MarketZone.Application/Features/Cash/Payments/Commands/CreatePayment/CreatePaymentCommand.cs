using System;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Features.Cash.Payments.Commands.CreatePayment
{
	public class CreatePaymentCommand : IRequest<BaseResult<long>>
	{
		public long? CashRegisterId { get; set; }
		public PaymentType PaymentType { get; set; } = PaymentType.SalesPayment;
		public long? InvoiceId { get; set; }
		public InvoiceType? InvoiceType { get; set; }
		public DateTime? PaymentDate { get; set; }
		public decimal Amount { get; set; }
		public string Currency { get; set; } = "SYP";           // عملة المورد/الفاتورة
		public string PaymentCurrency { get; set; } = "SYP";     // العملة المراد الدفع بها
		public decimal? ExchangeRate { get; set; }               // سعر الصرف
		public string Notes { get; set; }
		public string Description { get; set; }
		public string ReceivedBy { get; set; }
		public string PaidBy { get; set; }
		public bool IsConfirmed { get; set; } = true;
		public ReferenceType ReferenceType { get; set; } = ReferenceType.PurchaseInvoice;
	}
}


