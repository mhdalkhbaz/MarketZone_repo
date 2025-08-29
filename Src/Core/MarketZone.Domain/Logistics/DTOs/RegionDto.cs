using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Domain.Logistics.DTOs
{
    public class RegionDto
    {
        public RegionDto()
        {
        }

        public RegionDto(Region region)
        {
            Id = region.Id;
            Name = region.Name;
            Description = region.Description;
            IsActive = region.IsActive;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}


