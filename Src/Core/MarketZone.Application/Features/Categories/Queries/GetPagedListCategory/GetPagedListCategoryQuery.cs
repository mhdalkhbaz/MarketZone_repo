using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Categories.DTOs;

namespace MarketZone.Application.Features.Categories.Queries.GetPagedListCategory
{
	public class GetPagedListCategoryQuery : CategoryFilter, IRequest<PagedResponse<CategoryDto>>
	{
	}
}


