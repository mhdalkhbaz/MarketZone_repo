using System.Collections.Generic;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CreateSalesInvoices
{
	public class CreateSalesInvoicesFromTripCommand : IRequest<BaseResult<List<long>>>
	{
		public long TripId { get; set; }
		public List<CustomerSaleItem> CustomerSales { get; set; } = new List<CustomerSaleItem>();
	}

	public class CustomerSaleItem
	{
		public long CustomerId { get; set; }
		public List<CustomerSaleDetailItem> Details { get; set; } = new List<CustomerSaleDetailItem>();
	}

	public class CustomerSaleDetailItem
	{
		public long ProductId { get; set; }
		public decimal Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal SubTotal { get; set; }
		public string Notes { get; set; } = string.Empty;
	}
}
