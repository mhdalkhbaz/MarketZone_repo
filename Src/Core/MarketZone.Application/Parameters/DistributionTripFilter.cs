namespace MarketZone.Application.Parameters
{
    public class DistributionTripFilter : PaginationRequestParameter
    {
        public long? CarId { get; set; }
        public long? RegionId { get; set; }
        public string Name { get; set; } // Maps to TripNumber search
    }
}

