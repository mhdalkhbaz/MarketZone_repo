using System;
using System.Collections.Generic;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.Enums;

namespace MarketZone.Application.Features.Sales.Commands.CreateSalesInvoice
{
	public class CreateSalesInvoiceCommand : IRequest<BaseResult<long>>
	{
		public string InvoiceNumber { get; set; } = string.Empty;
		public long CustomerId { get; set; }
		public DateTime InvoiceDate { get; set; }
		public decimal TotalAmount { get; set; }
		public decimal Discount { get; set; }
		public string PaymentMethod { get; set; } = string.Empty;
		public string Notes { get; set; } = string.Empty;
		public string? Currency { get; set; }
		public SalesInvoiceType Type { get; set; } = SalesInvoiceType.Regular;
		public long? DistributionTripId { get; set; } // ربط برحلة التوزيع (اختياري)
		public List<CreateSalesInvoiceDetailItem> Details { get; set; } = new List<CreateSalesInvoiceDetailItem>();
	}

	public class CreateSalesInvoiceDetailItem
	{
		public long ProductId { get; set; }
		public decimal Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal SubTotal { get; set; }
		public string Notes { get; set; } = string.Empty;
	}
}



