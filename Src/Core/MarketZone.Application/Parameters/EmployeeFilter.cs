namespace MarketZone.Application.Parameters
{
    public class EmployeeFilter : PaginationRequestParameter
    {
        public string Name { get; set; }
        public string Address { get; set; } // Maps to Address
        public int? Status { get; set; } // Maps to IsActive: 1 = true, 0 = false
        public string? Type { get; set; }  
    }
}


