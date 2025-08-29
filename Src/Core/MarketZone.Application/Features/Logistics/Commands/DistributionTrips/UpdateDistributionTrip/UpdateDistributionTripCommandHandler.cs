using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.UpdateDistributionTrip
{
    public class UpdateDistributionTripCommandHandler(IDistributionTripRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateDistributionTripCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateDistributionTripCommand request, CancellationToken cancellationToken)
        {
            var entity = await repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
            if (entity is null)
                return BaseResult.Failure();

            mapper.Map(request, entity);

            if (request.Details != null)
            {
                var existing = entity.Details?.ToDictionary(d => d.Id) ?? new System.Collections.Generic.Dictionary<long, DistributionTripDetail>();
                foreach (var item in request.Details)
                {
                    if (item.IsDeleted && item.Id.HasValue && existing.TryGetValue(item.Id.Value, out var toRemove))
                    {
                        entity.Details.Remove(toRemove);
                        continue;
                    }
                    if (item.Id.HasValue && existing.TryGetValue(item.Id.Value, out var toUpdate))
                    {
                        toUpdate.Update(item.ProductId, item.Qty, item.ExpectedPrice);
                    }
                    else if (!item.IsDeleted)
                    {
                        entity.Details.Add(new DistributionTripDetail(entity.Id, item.ProductId, item.Qty, item.ExpectedPrice));
                    }
                }
            }

            await unitOfWork.SaveChangesAsync();
            return BaseResult.Ok();
        }
    }
}


