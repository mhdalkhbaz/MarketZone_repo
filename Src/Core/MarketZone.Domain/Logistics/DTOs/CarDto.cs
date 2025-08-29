using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Domain.Logistics.DTOs
{
    public class CarDto
    {
        public CarDto()
        {
        }

        public CarDto(Car car)
        {
            Id = car.Id;
            Name = car.Name;
            PlateNumber = car.PlateNumber;
            Model = car.Model;
            Year = car.Year;
            CapacityKg = car.CapacityKg;
            IsAvailable = car.IsAvailable;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string PlateNumber { get; set; }
        public string Model { get; set; }
        public int? Year { get; set; }
        public decimal? CapacityKg { get; set; }
        public bool IsAvailable { get; set; }
    }
}


