using MarketZone.Domain.Common;

namespace MarketZone.Domain.Categories.Entities
{
	public class Category : AuditableBaseEntity
	{
#pragma warning disable
		private Category()
		{
		}
#pragma warning restore
		public Category(string name, string description)
		{
			Name = name;
			Description = description;
		}

		public string Name { get; private set; }
		public string Description { get; private set; }

		public void Update(string name, string description)
		{
			Name = name;
			Description = description;
		}
	}
}


