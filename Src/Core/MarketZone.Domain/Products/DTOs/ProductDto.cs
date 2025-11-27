using System;
using MarketZone.Domain.Products.Enums;

namespace MarketZone.Domain.Products.DTOs
{
    public class ProductDto
    {
#pragma warning disable
        public ProductDto()
        {

        }
#pragma warning restore 
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal MinStockLevel { get; set; }
        public bool IsActive { get; set; }
        public bool NeedsRoasting { get; set; }
        public decimal? RoastingCost { get; set; }
        public decimal? CommissionPerKg { get; set; }
        public long? RawProductId { get; set; }
        public string BarCode { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public decimal Qty { get; set; }
        public decimal AvailableQty { get; set; }
        public decimal AverageCost { get; set; }
        public ProductType ProductType { get; set; }
    }
}
