using MarketZone.Domain.Categories.Entities;
using MarketZone.Domain.Common;
using MarketZone.Domain.Products.Enums;

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
            decimal minStockLevel,
            bool isActive,
            bool needsRoasting,
            string barCode,
            ProductType productType = ProductType.Ready)
        {
            CategoryId = categoryId;
            Name = name;
            Description = description;
            UnitOfMeasure = string.IsNullOrWhiteSpace(unitOfMeasure) ? "kg" : unitOfMeasure;
            MinStockLevel = minStockLevel == default ? 5 : minStockLevel;
            IsActive = isActive;
            NeedsRoasting = needsRoasting;
            BarCode = barCode;
            ProductType = productType;
        }

        public long CategoryId { get; private set; }
        public Category Category { get; private set; }
        public string BarCode { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public string UnitOfMeasure { get; private set; }
        public decimal MinStockLevel { get; private set; }
        public bool IsActive { get; private set; } = true;
      


        public bool NeedsRoasting { get; private set; } = false;
        public long? RawProductId { get; private set; }

        public decimal? CommissionPerKg { get; private set; }
        public ProductType ProductType { get; private set; } = ProductType.Ready;

        public void Update(
            long categoryId,
            string name,
            string description,
            string unitOfMeasure,
            decimal minStockLevel,
            bool isActive,
            bool needsRoasting,
            string barCode,
            ProductType? productType = null)
        {
            CategoryId = categoryId;
            Name = name;
            Description = description;
            UnitOfMeasure = string.IsNullOrWhiteSpace(unitOfMeasure) ? UnitOfMeasure : unitOfMeasure;
            MinStockLevel = minStockLevel;
            IsActive = isActive;
            NeedsRoasting = needsRoasting;
            BarCode = barCode;
            if (productType.HasValue)
            {
                ProductType = productType.Value;
            }
        }

        public void SetProductType(ProductType productType)
        {
            ProductType = productType;
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
