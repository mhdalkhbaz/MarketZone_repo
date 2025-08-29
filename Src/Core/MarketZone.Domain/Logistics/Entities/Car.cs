using MarketZone.Domain.Common;

namespace MarketZone.Domain.Logistics.Entities
{
	public class Car : AuditableBaseEntity
	{
		private Car()
		{
		}

		public Car(string name, string? plateNumber, string? model, int? year, decimal? capacityKg, bool isAvailable = true)
		{
			Name = name;
			PlateNumber = plateNumber;
			Model = model;
			Year = year;
			CapacityKg = capacityKg;
			IsAvailable = isAvailable;
		}

		public string Name { get; private set; }
		public string? PlateNumber { get; private set; }
		public string? Model { get; private set; }
		public int? Year { get; private set; }
		public decimal? CapacityKg { get; private set; }
		public bool IsAvailable { get; private set; } = true;

		public void Update(string name, string? plateNumber, string? model, int? year, decimal? capacityKg, bool isAvailable)
		{
			Name = name;
			PlateNumber = plateNumber;
			Model = model;
			Year = year;
			CapacityKg = capacityKg;
			IsAvailable = isAvailable;
		}
	}
}


