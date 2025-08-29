using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Categories.DTOs;

namespace MarketZone.Application.Features.Categories.Queries.GetCategoryById
{
	public class GetCategoryByIdQuery : IRequest<BaseResult<CategoryDto>>
	{
		public long Id { get; set; }
	}
}


