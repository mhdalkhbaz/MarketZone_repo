using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<BaseResult<long>>
    {
        public long? CategoryId { get; set; } = 1;
        public string Name { get; set; }
        public string Description { get; set; } = "";
        public string UnitOfMeasure { get; set; } = "kg";
        public decimal? PurchasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal MinStockLevel { get; set; } = 5;
        public bool IsActive { get; set; } = true;
        public bool NeedsRoasting { get; set; } = false;
        public decimal? RoastingCost { get; set; }
        public string BarCode { get; set; } = "";
        public long? RawProductId { get; set; }
        public decimal? CommissionPerKg { get; set; }
    }
}
