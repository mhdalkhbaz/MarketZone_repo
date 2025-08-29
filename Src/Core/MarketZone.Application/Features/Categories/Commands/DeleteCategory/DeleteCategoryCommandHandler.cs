using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Categories.Commands.DeleteCategory
{
	public class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<DeleteCategoryCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = await categoryRepository.GetByIdAsync(request.Id);

			if (category is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CategoryMessages.Category_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			categoryRepository.Delete(category);
			await unitOfWork.SaveChangesAsync();

			return BaseResult.Ok();
		}
	}
}


