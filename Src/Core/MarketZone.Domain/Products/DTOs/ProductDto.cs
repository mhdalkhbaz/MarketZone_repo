using System;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Domain.Products.DTOs
{
    public class ProductDto
    {
#pragma warning disable
        public ProductDto()
        {

        }
#pragma warning restore 
        public ProductDto(Product product)
        {
            Id = product.Id;
            CategoryId = product.CategoryId;
            Name = product.Name;
            Description = product.Description;
            UnitOfMeasure = product.UnitOfMeasure;
            PurchasePrice = product.PurchasePrice;
            SalePrice = product.SalePrice;
            MinStockLevel = product.MinStockLevel;
            IsActive = product.IsActive;
            NeedsRoasting = product.NeedsRoasting;
            RoastingCost = product.RoastingCost;
            BarCode = product.BarCode;
            CreatedDateTime = product.Created;
        }
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
        public string BarCode { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
