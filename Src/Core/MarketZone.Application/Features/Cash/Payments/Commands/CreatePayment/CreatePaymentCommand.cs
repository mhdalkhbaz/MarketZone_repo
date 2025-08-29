using System;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Features.Cash.Payments.Commands.CreatePayment
{
	public class CreatePaymentCommand : IRequest<BaseResult<long>>
	{
		public long? CashRegisterId { get; set; }
		public long InvoiceId { get; set; }
		public DateTime? PaymentDate { get; set; }
		public decimal Amount { get; set; }
		public string Notes { get; set; }
		public string ReceivedBy { get; set; }
		public bool IsConfirmed { get; set; } = true;
		public ReferenceType ReferenceType { get; set; } = ReferenceType.PurchaseInvoice;
	}
}


