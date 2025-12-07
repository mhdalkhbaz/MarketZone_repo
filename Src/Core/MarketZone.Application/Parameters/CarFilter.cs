namespace MarketZone.Application.Parameters
{
    public class CarFilter : PaginationRequestParameter
    {
        public string Name { get; set; }
        public string Model { get; set; }
        public int? Status { get; set; } // Maps to IsAvailable: 1 = true, 0 = false
    }
}


