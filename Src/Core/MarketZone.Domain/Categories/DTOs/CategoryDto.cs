using System;
using MarketZone.Domain.Categories.Entities;

namespace MarketZone.Domain.Categories.DTOs
{
	public class CategoryDto
	{
#pragma warning disable
		public CategoryDto()
		{
		}
#pragma warning restore
		public CategoryDto(Category category)
		{
			Id = category.Id;
			Name = category.Name;
			Description = category.Description;
			CreatedDateTime = category.Created;
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime CreatedDateTime { get; set; }
	}
}


