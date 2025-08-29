using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Categories.DTOs;

namespace MarketZone.Application.Features.Categories.Queries.GetCategoryById
{
	public class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, ITranslator translator, IMapper mapper) : IRequestHandler<GetCategoryByIdQuery, BaseResult<CategoryDto>>
	{
		public async Task<BaseResult<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
		{
			var category = await categoryRepository.GetByIdAsync(request.Id);

			if (category is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CategoryMessages.Category_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			return mapper.Map<CategoryDto>(category);
		}
	}
}


