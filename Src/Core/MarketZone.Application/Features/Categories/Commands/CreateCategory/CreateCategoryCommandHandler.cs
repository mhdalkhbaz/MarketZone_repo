using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Categories.Entities;

namespace MarketZone.Application.Features.Categories.Commands.CreateCategory
{
	public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateCategoryCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = mapper.Map<Category>(request);

			await categoryRepository.AddAsync(category);
			await unitOfWork.SaveChangesAsync();

			return category.Id;
		}
	}
}


