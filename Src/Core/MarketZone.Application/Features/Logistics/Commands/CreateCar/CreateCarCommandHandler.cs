using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Application.Features.Logistics.Commands.CreateCar
{
	public class CreateCarCommandHandler(ICarRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateCarCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreateCarCommand request, CancellationToken cancellationToken)
		{
			var entity = mapper.Map<Car>(request);
			await repository.AddAsync(entity);
			await unitOfWork.SaveChangesAsync();
			return entity.Id;
		}
	}
}


