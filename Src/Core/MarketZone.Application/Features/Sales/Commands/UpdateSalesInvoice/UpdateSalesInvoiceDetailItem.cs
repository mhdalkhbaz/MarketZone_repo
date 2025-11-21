namespace MarketZone.Application.Features.Sales.Commands.UpdateSalesInvoice
{
	public class UpdateSalesInvoiceDetailItem
	{
		public long? Id { get; set; }
		public long ProductId { get; set; }
		public decimal Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal SubTotal { get; set; }
		public string Notes { get; set; } = string.Empty;
		public bool IsDeleted { get; set; }
	}
}


