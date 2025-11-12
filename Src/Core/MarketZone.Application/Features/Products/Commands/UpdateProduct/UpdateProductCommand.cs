using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommand : IRequest<BaseResult>
    {
        public long Id { get; set; }
        public long CategoryId { get; set; } = 1;
        public string Name { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal? MinStockLevel { get; set; }
        public bool? IsActive { get; set; }
        public bool? NeedsRoasting { get; set; }
        public string BarCode { get; set; }
        public decimal? CommissionPerKg { get; set; }
    }
}
