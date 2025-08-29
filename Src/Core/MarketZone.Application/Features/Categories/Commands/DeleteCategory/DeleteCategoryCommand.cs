using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Categories.Commands.DeleteCategory
{
	public class DeleteCategoryCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
	}
}


