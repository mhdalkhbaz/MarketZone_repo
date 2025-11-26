namespace MarketZone.Application.Parameters
{
    public class ProductFilter : PaginationRequestParameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; } // Maps to IsActive: 1 = true, 0 = false
        public int? Type { get; set; } // Maps to ProductType enum
    }
}


