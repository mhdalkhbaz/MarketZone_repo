namespace MarketZone.Domain.Products.DTOs
{
    public class CompositeProductDetailDto
    {
        public long Id { get; set; }
        public long ComponentProductId { get; set; }
        public string ComponentProductName { get; set; }
        public decimal Quantity { get; set; }
    }
}

