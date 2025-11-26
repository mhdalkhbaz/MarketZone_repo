namespace MarketZone.Application.Parameters
{
    public class CategoryFilter : PaginationRequestParameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}


