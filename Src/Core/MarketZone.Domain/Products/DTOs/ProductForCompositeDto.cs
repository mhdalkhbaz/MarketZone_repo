namespace MarketZone.Domain.Products.DTOs
{
    public class ProductForCompositeDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Qty { get; set; }
        public decimal SalePrice { get; set; }
        public decimal? CommissionPerKg { get; set; }
    }
}

