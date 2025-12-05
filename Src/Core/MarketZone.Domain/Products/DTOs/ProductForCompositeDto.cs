namespace MarketZone.Domain.Products.DTOs
{
    public class ProductForCompositeDto
    {
        public long resultingProductId { get; set; }
        public string resultingProductName { get; set; }
        public decimal Qty { get; set; }
        public decimal SalePrice { get; set; }
        public decimal? CommissionPerKg { get; set; }
    }
}

