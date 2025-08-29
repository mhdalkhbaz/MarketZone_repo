using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Categories.Commands.UpdateCategory
{
	public class UpdateCategoryCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}


