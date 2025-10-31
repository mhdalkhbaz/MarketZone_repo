using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Domain.Cash.DTOs
{
    public class CustomerSelectListDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Currency? Currency { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
