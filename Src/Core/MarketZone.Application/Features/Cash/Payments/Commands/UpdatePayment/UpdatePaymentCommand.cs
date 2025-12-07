using System;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash.Payments.Commands.UpdatePayment
{
	public class UpdatePaymentCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
		public long? CashRegisterId { get; set; }
		public DateTime? PaymentDate { get; set; }
		public decimal? Amount { get; set; }
		public string Notes { get; set; }
		public string ReceivedBy { get; set; }
		public string PaidBy { get; set; }
		public string Description { get; set; }
		public Domain.Cash.Enums.Currency? Currency { get; set; }
		public Domain.Cash.Enums.Currency? PaymentCurrency { get; set; }
		public decimal? ExchangeRate { get; set; }
		public bool? IsConfirmed { get; set; }
		public PaymentStatus? Status { get; set; }
	}
}


