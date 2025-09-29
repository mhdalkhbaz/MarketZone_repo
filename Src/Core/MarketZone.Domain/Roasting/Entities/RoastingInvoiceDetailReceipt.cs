using MarketZone.Domain.Common;

namespace MarketZone.Domain.Roasting.Entities
{
    public class RoastingInvoiceDetailReceipt : AuditableBaseEntity
    {
        private RoastingInvoiceDetailReceipt()
        {
        }

        public RoastingInvoiceDetailReceipt(
            long roastingInvoiceId,
            long detailId,
            long readyProductId,
            decimal quantityKg,
            decimal salePricePerKg,
            decimal roastingCostPerKg,
            decimal commissionPerKg,
            decimal netSalePricePerKg)
        {
            RoastingInvoiceId = roastingInvoiceId;
            DetailId = detailId;
            ReadyProductId = readyProductId;
            QuantityKg = quantityKg;
            SalePricePerKg = salePricePerKg;
            RoastingCostPerKg = roastingCostPerKg;
            CommissionPerKg = commissionPerKg;
            NetSalePricePerKg = netSalePricePerKg;
        }

        public long RoastingInvoiceId { get; private set; }
        public long DetailId { get; private set; }
        public long ReadyProductId { get; private set; }
        public decimal QuantityKg { get; private set; }
        public decimal SalePricePerKg { get; private set; }
        public decimal RoastingCostPerKg { get; private set; }
        public decimal CommissionPerKg { get; private set; }
        public decimal NetSalePricePerKg { get; private set; }

        public decimal TotalSalePrice => QuantityKg * SalePricePerKg;
        public decimal TotalRoastingCost => QuantityKg * RoastingCostPerKg;
        public decimal TotalCommission => QuantityKg * CommissionPerKg;
        public decimal TotalNetSalePrice => QuantityKg * NetSalePricePerKg;

        public RoastingInvoiceDetail Detail { get; private set; }
    }
}


