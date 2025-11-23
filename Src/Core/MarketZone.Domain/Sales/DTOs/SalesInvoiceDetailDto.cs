using MarketZone.Domain.Sales.Entities;

namespace MarketZone.Domain.Sales.DTOs
{
	public class SalesInvoiceDetailDto
	{
#pragma warning disable
		public SalesInvoiceDetailDto()
		{
		}
#pragma warning restore
		public SalesInvoiceDetailDto(SalesInvoiceDetail detail)
		{
			Id = detail.Id;
			ProductId = detail.ProductId;
			ProductName = detail.Product?.Name ?? string.Empty;
			Quantity = detail.Quantity;
			UnitPrice = detail.UnitPrice;
			SubTotal = detail.SubTotal;
			Notes = detail.Notes;
		}

		public long Id { get; set; }
		public long ProductId { get; set; }
		public string ProductName { get; set; }
		public decimal Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal SubTotal { get; set; }
		public string Notes { get; set; }
	}
}



