using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CreateDistributionTrip
{
	public class CreateDistributionTripCommandHandler(IDistributionTripRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateDistributionTripCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreateDistributionTripCommand request, CancellationToken cancellationToken)
		{
			var entity = mapper.Map<DistributionTrip>(request);
			await repository.AddAsync(entity);
			await unitOfWork.SaveChangesAsync();
			return entity.Id;
		}
	}
}


