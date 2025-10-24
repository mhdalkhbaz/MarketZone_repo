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
		public long? InvoiceId { get; set; }  // ربط بفاتورة (اختياري)
		public DateTime? PaymentDate { get; set; }
		public decimal Amount { get; set; }
		public string Currency { get; set; } = "SYP";
		public decimal? ExchangeRate { get; set; }
		public string Notes { get; set; }
		public string Description { get; set; }
		public string ReceivedBy { get; set; }
		public string PaidBy { get; set; }
		public bool IsConfirmed { get; set; } = true;
	}
}


