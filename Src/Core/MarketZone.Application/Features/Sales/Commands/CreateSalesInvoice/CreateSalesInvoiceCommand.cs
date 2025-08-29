using System;
using System.Collections.Generic;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Sales.Commands.CreateSalesInvoice
{
	public class CreateSalesInvoiceCommand : IRequest<BaseResult<long>>
	{
		public string InvoiceNumber { get; set; }
		public long? CustomerId { get; set; }
		public DateTime? InvoiceDate { get; set; }
		public decimal TotalAmount { get; set; }
		public decimal Discount { get; set; }
		public string PaymentMethod { get; set; }
		public string Notes { get; set; }
		public List<CreateSalesInvoiceDetailItem> Details { get; set; } = [];
	}

	public class CreateSalesInvoiceDetailItem
	{
		public long ProductId { get; set; }
		public decimal Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal SubTotal { get; set; }
		public string Notes { get; set; }
	}
}



