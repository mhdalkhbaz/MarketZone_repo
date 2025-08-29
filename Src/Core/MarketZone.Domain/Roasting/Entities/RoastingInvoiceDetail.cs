using System;
using MarketZone.Domain.Common;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Domain.Roasting.Entities
{
    public class RoastingInvoiceDetail : AuditableBaseEntity
    {
        private RoastingInvoiceDetail() { }

        public RoastingInvoiceDetail(long roastingInvoiceId, long productId, decimal quantityKg, decimal roastPricePerKg, decimal actualQuantityAfterRoasting, string notes)
        {
            RoastingInvoiceId = roastingInvoiceId;
            ProductId = productId;
            QuantityKg = quantityKg;
            RoastPricePerKg = roastPricePerKg;
            ActualQuantityAfterRoasting = actualQuantityAfterRoasting;
            Notes = notes;
            TotalPrice = quantityKg * roastPricePerKg;
        }

        public long RoastingInvoiceId { get; private set; }
        public long ProductId { get; private set; }
        public decimal QuantityKg { get; private set; }
        public decimal RoastPricePerKg { get; private set; }
        public decimal ActualQuantityAfterRoasting { get; private set; }
        public decimal TotalPrice { get; private set; }
        public string Notes { get; private set; }

        // Navigation properties
        public virtual RoastingInvoice RoastingInvoice { get; private set; }
        public virtual Product Product { get; private set; }
    }
}
