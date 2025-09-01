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
			PaymentDate = e.PaymentDate;
			Amount = e.Amount;
			Notes = e.Notes;
			Description = e.Description;
			ReceivedBy = e.ReceivedBy;
			PaidBy = e.PaidBy;
			IsConfirmed = e.IsConfirmed;
			Status = e.Status;
		}

		public long Id { get; set; }
		public long? CashRegisterId { get; set; }
		public PaymentType PaymentType { get; set; }
		public long? InvoiceId { get; set; }
		public DateTime PaymentDate { get; set; }
		public decimal Amount { get; set; }
		public string Notes { get; set; }
		public string Description { get; set; }
		public string ReceivedBy { get; set; }
		public string PaidBy { get; set; }
		public bool IsConfirmed { get; set; }
		public PaymentStatus Status { get; set; }
	}
}


