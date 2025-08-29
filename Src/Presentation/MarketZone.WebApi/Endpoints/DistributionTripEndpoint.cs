using MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CreateDistributionTrip;
using MarketZone.Application.Features.Logistics.Commands.DistributionTrips.UpdateDistributionTrip;
using MarketZone.Application.Features.Logistics.Queries.GetPagedListDistributionTrip;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace MarketZone.WebApi.Endpoints
{
    public class DistributionTripEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedListDistributionTrip);
            builder.MapPost(CreateDistributionTrip).RequireAuthorization();
            builder.MapPut(UpdateDistributionTrip).RequireAuthorization();
        }

        async Task<PagedResponse<DistributionTripDto>> GetPagedListDistributionTrip(IMediator mediator, [AsParameters] GetPagedListDistributionTripQuery model)
            => await mediator.Send<GetPagedListDistributionTripQuery, PagedResponse<DistributionTripDto>>(model);

        async Task<BaseResult<long>> CreateDistributionTrip(IMediator mediator, CreateDistributionTripCommand model)
            => await mediator.Send<CreateDistributionTripCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdateDistributionTrip(IMediator mediator, UpdateDistributionTripCommand model)
            => await mediator.Send<UpdateDistributionTripCommand, BaseResult>(model);
    }
}


