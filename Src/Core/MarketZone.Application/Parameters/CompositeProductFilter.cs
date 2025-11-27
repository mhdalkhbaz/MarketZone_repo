namespace MarketZone.Application.Parameters
{
    public class CompositeProductFilter : PaginationRequestParameter
    {
        public string ResultingProductName { get; set; }
        public int? Status { get; set; } // Maps to CompositeProductStatus: 0 = Draft, 1 = Posted
        public long? ResultingProductId { get; set; }
    }
}

