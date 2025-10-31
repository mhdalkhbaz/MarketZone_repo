using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Domain.Cash.DTOs
{
    public class SupplierSelectListDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Currency? Currency { get; set; }
    }
}
