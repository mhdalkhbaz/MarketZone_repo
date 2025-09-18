using MarketZone.Domain.Categories.Entities;
using MarketZone.Domain.Common;

namespace MarketZone.Domain.Products.Entities
{
    public class Product : AuditableBaseEntity
    {
        private Product()
        {
        }
        public Product(
            long categoryId,
            string name,
            string description,
            string unitOfMeasure,
            decimal? purchasePrice,
            decimal? salePrice,
            decimal minStockLevel,
            bool isActive,
            bool needsRoasting,
            decimal? roastingCost,
            string barCode)
        {
            CategoryId = categoryId;
            Name = name;
            Description = description;
            UnitOfMeasure = string.IsNullOrWhiteSpace(unitOfMeasure) ? "kg" : unitOfMeasure;
            PurchasePrice = purchasePrice;
            SalePrice = salePrice;
            MinStockLevel = minStockLevel == default ? 5 : minStockLevel;
            IsActive = isActive;
            NeedsRoasting = needsRoasting;
            RoastingCost = roastingCost;
            BarCode = barCode;
        }

        public long CategoryId { get; private set; }
        public Category Category { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string UnitOfMeasure { get; private set; }
        public decimal? PurchasePrice { get; private set; }
        public decimal? SalePrice { get; private set; }
        public decimal MinStockLevel { get; private set; }
        public bool IsActive { get; private set; } = true;
        public bool NeedsRoasting { get; private set; } = false;
        public decimal? RoastingCost { get; private set; }
        public string BarCode { get; private set; }
        public string BarCode2 { get; private set; }
        public long? RawProductId { get; private set; }
        public decimal? CommissionPerKg { get; private set; }

        public void Update(
            long categoryId,
            string name,
            string description,
            string unitOfMeasure,
            decimal? purchasePrice,
            decimal? salePrice,
            decimal minStockLevel,
            bool isActive,
            bool needsRoasting,
            decimal? roastingCost,
            string barCode)
        {
            CategoryId = categoryId;
            Name = name;
            Description = description;
            UnitOfMeasure = string.IsNullOrWhiteSpace(unitOfMeasure) ? UnitOfMeasure : unitOfMeasure;
            PurchasePrice = purchasePrice;
            SalePrice = salePrice;
            MinStockLevel = minStockLevel;
            IsActive = isActive;
            NeedsRoasting = needsRoasting;
            RoastingCost = roastingCost;
            BarCode = barCode;
        }

        public void SetRawProduct(long? rawProductId)
        {
            RawProductId = rawProductId;
        }

        public void SetCommissionPerKg(decimal? commissionPerKg)
        {
            CommissionPerKg = commissionPerKg;
        }
    }
}
