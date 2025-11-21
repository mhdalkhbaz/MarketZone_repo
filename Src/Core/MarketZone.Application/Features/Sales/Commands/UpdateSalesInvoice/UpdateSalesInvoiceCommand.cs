using System;
using System.Collections.Generic;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Features.Sales.Commands.UpdateSalesInvoice
{
	public class UpdateSalesInvoiceCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
		public string InvoiceNumber { get; set; }
		public long? CustomerId { get; set; }
		public DateTime? InvoiceDate { get; set; }
		public decimal? TotalAmount { get; set; }
		public decimal? Discount { get; set; }
		public string PaymentMethod { get; set; }
		public string Notes { get; set; }
		public Currency? Currency { get; set; }
		public List<UpdateSalesInvoiceDetailItem> Details { get; set; }
	}
}



