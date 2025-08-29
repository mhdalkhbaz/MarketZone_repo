using MarketZone.Domain.Common;

namespace MarketZone.Domain.Logistics.Entities
{
	public class Region : AuditableBaseEntity
	{
		private Region()
		{
		}

		public Region(string name, string description, bool isActive = true)
		{
			Name = name;
			Description = description;
			IsActive = isActive;
		}

		public string Name { get; private set; }
		public string Description { get; private set; }
		public bool IsActive { get; private set; } = true;

		public void Update(string name, string description, bool isActive)
		{
			Name = name;
			Description = description;
			IsActive = isActive;
		}
	}
}


