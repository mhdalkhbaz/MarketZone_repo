using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Categories.Commands.CreateCategory
{
	public class CreateCategoryCommand : IRequest<BaseResult<long>>
	{
		public string Name { get; set; }
		public string Description { get; set; }
	}
}


