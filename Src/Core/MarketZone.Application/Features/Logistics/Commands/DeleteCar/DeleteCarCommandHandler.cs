using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.DeleteCar
{
	public class DeleteCarCommandHandler(ICarRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteCarCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetByIdAsync(request.Id);
			if (entity is null)
				return BaseResult.Failure();

			repository.Delete(entity);
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}


