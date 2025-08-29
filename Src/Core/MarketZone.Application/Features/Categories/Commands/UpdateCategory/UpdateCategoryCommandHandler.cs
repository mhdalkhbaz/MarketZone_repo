using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Categories.Commands.UpdateCategory
{
	public class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, ITranslator translator, IMapper mapper) : IRequestHandler<UpdateCategoryCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = await categoryRepository.GetByIdAsync(request.Id);

			if (category is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CategoryMessages.Category_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			mapper.Map(request, category);
			await unitOfWork.SaveChangesAsync();

			return BaseResult.Ok();
		}
	}
}


