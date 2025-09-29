using System.Collections.Generic;
using System.Linq;
using MarketZone.Domain.Common;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Domain.Roasting.Entities
{
    public class RoastingInvoiceDetail : AuditableBaseEntity
    {
        private RoastingInvoiceDetail() { }

        public RoastingInvoiceDetail(long roastingInvoiceId, long rawProductId, decimal quantityKg, string notes)
        {
            RoastingInvoiceId = roastingInvoiceId;
            RawProductId = rawProductId;
            QuantityKg = quantityKg;
            Notes = notes ?? string.Empty;
            ReceivedQuantityKg = 0;
        }

        public long RoastingInvoiceId { get; private set; }
        public long RawProductId { get; private set; }
        public decimal QuantityKg { get; private set; }
        public decimal ReceivedQuantityKg { get; private set; }
        public string Notes { get; private set; }

        public decimal RemainingQuantity => QuantityKg - ReceivedQuantityKg;

        // Navigation properties
        public virtual RoastingInvoice RoastingInvoice { get; private set; }
        public virtual Product RawProduct { get; private set; }
        public virtual ICollection<RoastingInvoiceDetailReceipt> Receipts { get; private set; } = new List<RoastingInvoiceDetailReceipt>();

        public void Update(long rawProductId, decimal quantityKg, string notes)
        {
            RawProductId = rawProductId;
            QuantityKg = quantityKg;
            Notes = notes ?? string.Empty;
        }

        public void SetReceivedQuantity(decimal totalReceived)
        {
            ReceivedQuantityKg = totalReceived;
        }

        public void ClearReceipts()
        {
            Receipts?.Clear();
            ReceivedQuantityKg = 0;
        }

        public void AddReceipt(RoastingInvoiceDetailReceipt receipt)
        {
            Receipts ??= new List<RoastingInvoiceDetailReceipt>();
            Receipts.Add(receipt);
            ReceivedQuantityKg = Receipts.Sum(r => r.QuantityKg);
        }

        public void RecalculateReceivedQuantity()
        {
            ReceivedQuantityKg = Receipts?.Sum(r => r.QuantityKg) ?? 0;
        }
    }
}
