using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.UpdateCar
{
	public class UpdateCarCommandHandler(ICarRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateCarCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(UpdateCarCommand request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetByIdAsync(request.Id);
			if (entity is null)
			{
				return BaseResult.Failure();
			}
			mapper.Map(request, entity);
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}


