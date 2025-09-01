using MarketZone.Domain.Common;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Domain.Sales.Entities
{
	public class SalesInvoiceDetail : AuditableBaseEntity
	{
#pragma warning disable
		private SalesInvoiceDetail()
		{
		}
#pragma warning restore
		public SalesInvoiceDetail(long invoiceId, long productId, decimal quantity, decimal unitPrice, decimal subTotal, string notes)
		{
			InvoiceId = invoiceId;
			ProductId = productId;
			Quantity = quantity;
			UnitPrice = unitPrice;
			SubTotal = subTotal;
			Notes = notes ?? string.Empty;
		}

		public SalesInvoiceDetail(long productId, decimal quantity, decimal unitPrice, decimal subTotal, string notes)
		{
			InvoiceId = 0; // Will be set later
			ProductId = productId;
			Quantity = quantity;
			UnitPrice = unitPrice;
			SubTotal = subTotal;
			Notes = notes ?? string.Empty;
		}

		public long InvoiceId { get; private set; }
		public SalesInvoice Invoice { get; private set; }
		public long ProductId { get; private set; }
		public Product Product { get; private set; }
		public decimal Quantity { get; private set; }
		public decimal UnitPrice { get; private set; }
		public decimal SubTotal { get; private set; }
		public string Notes { get; private set; }

		public void Update(long productId, decimal quantity, decimal unitPrice, decimal subTotal, string notes)
		{
			ProductId = productId;
			Quantity = quantity;
			UnitPrice = unitPrice;
			SubTotal = subTotal;
			Notes = notes ?? string.Empty;
		}

		public void SetInvoice(SalesInvoice invoice)
		{
			Invoice = invoice;
			InvoiceId = invoice.Id;
		}
	}
}



