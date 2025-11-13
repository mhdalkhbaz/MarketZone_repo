using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Domain.Cash.DTOs
{
    public class EmployeeSelectListDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Currency? Currency { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string JobTitle { get; set; }
    }
}

