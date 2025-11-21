using System;
using System.Collections.Generic;
using System.Linq;
using MarketZone.Domain.Sales.Entities;
using MarketZone.Domain.Sales.Enums;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Domain.Sales.DTOs
{
	public class SalesInvoiceDto
	{
		public SalesInvoiceDto()
		{
		}

	public SalesInvoiceDto(SalesInvoice invoice)
	{
		Id = invoice.Id;
		InvoiceNumber = invoice.InvoiceNumber;
		CustomerId = invoice.CustomerId;
		CustomerName = invoice.Customer?.Name ?? string.Empty;
		InvoiceDate = invoice.InvoiceDate;
		TotalAmount = invoice.TotalAmount;
		Discount = invoice.Discount;
		PaymentMethod = invoice.PaymentMethod;
		Notes = invoice.Notes;
		Currency = invoice.Currency;
		Status = invoice.Status;
		Type = invoice.Type;
		DistributionTripId = invoice.DistributionTripId;
		CreatedDateTime = invoice.Created;
		Details = invoice.Details?.Select(d => new SalesInvoiceDetailDto(d)).ToList() ?? new List<SalesInvoiceDetailDto>();
	}

	public long Id { get; set; }
	public string InvoiceNumber { get; set; } = string.Empty;
	public long CustomerId { get; set; }
	public string CustomerName { get; set; }
	public DateTime InvoiceDate { get; set; }
	public decimal TotalAmount { get; set; }
	public decimal Discount { get; set; }
	public string PaymentMethod { get; set; } = string.Empty;
	public string Notes { get; set; } = string.Empty;
	public Currency? Currency { get; set; }
	public SalesInvoiceStatus Status { get; set; }
	public SalesInvoiceType Type { get; set; }
	public long? DistributionTripId { get; set; }
	public DateTime CreatedDateTime { get; set; }
	public decimal PaidAmount { get; set; }
	public decimal UnpaidAmount { get; set; }
	public List<SalesInvoiceDetailDto> Details { get; set; } = new List<SalesInvoiceDetailDto>();
	}
}



