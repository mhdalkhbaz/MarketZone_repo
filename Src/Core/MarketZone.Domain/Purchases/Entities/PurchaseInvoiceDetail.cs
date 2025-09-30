using MarketZone.Domain.Common;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Domain.Purchases.Entities
{
    public class PurchaseInvoiceDetail : AuditableBaseEntity
    {
#pragma warning disable
        private PurchaseInvoiceDetail()
        {
        }
#pragma warning restore
        public PurchaseInvoiceDetail(long invoiceId, long productId, decimal quantity, decimal unitPrice, decimal totalPrice, string notes)
        {
            InvoiceId = invoiceId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalPrice = totalPrice;
            Notes = notes;
        }

        public long InvoiceId { get; private set; }
        public PurchaseInvoice Invoice { get; private set; }
        public long ProductId { get; private set; }
        public Product Product { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice { get; private set; }
        public string? Notes { get; private set; }

        public void Update(long productId, decimal quantity, decimal unitPrice, decimal totalPrice, string batchNumber, string notes)
        {
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalPrice = totalPrice;
            Notes = notes;
        }
    }
}



