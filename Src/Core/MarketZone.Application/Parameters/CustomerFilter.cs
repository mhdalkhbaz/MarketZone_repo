namespace MarketZone.Application.Parameters
{
    public class CustomerFilter : PaginationRequestParameter
    {
        public string Name { get; set; }
        public string Description { get; set; } // Maps to Address
        public int? Status { get; set; } // Maps to IsActive: 1 = true, 0 = false
    }
}


