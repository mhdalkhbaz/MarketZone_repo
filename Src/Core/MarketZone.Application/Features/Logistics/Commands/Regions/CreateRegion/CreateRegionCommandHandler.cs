using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Application.Features.Logistics.Commands.Regions.CreateRegion
{
	public class CreateRegionCommandHandler(IRegionRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateRegionCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreateRegionCommand request, CancellationToken cancellationToken)
		{
			var entity = mapper.Map<Region>(request);
			await repository.AddAsync(entity);
			await unitOfWork.SaveChangesAsync();
			return entity.Id;
		}
	}
}


