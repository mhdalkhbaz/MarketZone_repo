using System;
using MarketZone.Domain.Cash.Entities;

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
			InvoiceId = e.InvoiceId;
			PaymentDate = e.PaymentDate;
			Amount = e.Amount;
			Notes = e.Notes;
			ReceivedBy = e.ReceivedBy;
			IsConfirmed = e.IsConfirmed;
			Status = e.Status;
		}

		public long Id { get; set; }
		public long? CashRegisterId { get; set; }
		public long InvoiceId { get; set; }
		public DateTime PaymentDate { get; set; }
		public decimal Amount { get; set; }
		public string Notes { get; set; }
		public string ReceivedBy { get; set; }
		public bool IsConfirmed { get; set; }
		public PaymentStatus Status { get; set; }
	}
}


