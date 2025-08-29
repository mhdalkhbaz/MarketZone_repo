using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Categories.DTOs;

namespace MarketZone.Application.Features.Categories.Queries.GetPagedListCategory
{
	public class GetPagedListCategoryQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetPagedListCategoryQuery, PagedResponse<CategoryDto>>
	{
		public async Task<PagedResponse<CategoryDto>> Handle(GetPagedListCategoryQuery request, CancellationToken cancellationToken)
		{
			return await categoryRepository.GetPagedListAsync(request.PageNumber, request.PageSize, request.Name);
		}
	}
}


