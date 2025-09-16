using System;
using MarketZone.Domain.Common;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Domain.Roasting.Entities
{
    public class RoastingInvoiceDetail : AuditableBaseEntity
    {
        private RoastingInvoiceDetail() { }

        public RoastingInvoiceDetail(long roastingInvoiceId, long readyProductId, long? rawProductId, decimal quantityKg, decimal roastPricePerKg, decimal? commissionPerKg, decimal actualQuantityAfterRoasting, string notes)
        {
            RoastingInvoiceId = roastingInvoiceId;
            ReadyProductId = readyProductId;
            RawProductId = rawProductId;
            QuantityKg = quantityKg;
            RoastPricePerKg = roastPricePerKg;
            CommissionPerKg = commissionPerKg ?? 0;
            ActualQuantityAfterRoasting = actualQuantityAfterRoasting;
            Notes = notes;
            TotalPrice = quantityKg * roastPricePerKg;
        }

        public long RoastingInvoiceId { get; private set; }
        public long ReadyProductId { get; private set; }
        public long? RawProductId { get; private set; }
        public decimal QuantityKg { get; private set; }
        public decimal RoastPricePerKg { get; private set; }
        public decimal CommissionPerKg { get; private set; }
        public decimal ActualQuantityAfterRoasting { get; private set; }
        public decimal TotalPrice { get; private set; }
        public string Notes { get; private set; }

        // Navigation properties
        public virtual RoastingInvoice RoastingInvoice { get; private set; }
        public virtual Product ReadyProduct { get; private set; }
        public virtual Product RawProduct { get; private set; }
    }
}
