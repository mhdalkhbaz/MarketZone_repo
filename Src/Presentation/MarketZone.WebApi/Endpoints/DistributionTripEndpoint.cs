using System.Collections.Generic;
using MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CreateDistributionTrip;
using MarketZone.Application.Features.Logistics.Commands.DistributionTrips.UpdateDistributionTrip;
using MarketZone.Application.Features.Logistics.Commands.DistributionTrips.PostDistributionTrip;
using MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CompleteTrip;
using MarketZone.Application.Features.Logistics.Commands.DistributionTrips.DeleteDistributionTrip;
using MarketZone.Application.Features.Logistics.Queries.GetPagedListDistributionTrip;
using MarketZone.Application.Features.Logistics.Queries.ValidateTripQuantities;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using MarketZone.Application.Features.Logistics.Commands.DistributionTrips.ReceiveGoods;

namespace MarketZone.WebApi.Endpoints
{
    public class DistributionTripEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedListDistributionTrip);
            builder.MapGet(ValidateTripQuantities);
            builder.MapPost(CreateDistributionTrip).RequireAuthorization();
            builder.MapPut(UpdateDistributionTrip).RequireAuthorization();
            builder.MapPut(PostDistributionTrip).RequireAuthorization();
            builder.MapPut(ReceiveGoods).RequireAuthorization();
            builder.MapDelete(DeleteDistributionTrip).RequireAuthorization();
        }

        async Task<PagedResponse<DistributionTripDto>> GetPagedListDistributionTrip(IMediator mediator, [AsParameters] GetPagedListDistributionTripQuery model)
            => await mediator.Send<GetPagedListDistributionTripQuery, PagedResponse<DistributionTripDto>>(model);

        async Task<BaseResult<ValidateTripQuantitiesResult>> ValidateTripQuantities(IMediator mediator, [AsParameters] ValidateTripQuantitiesQuery model)
            => await mediator.Send<ValidateTripQuantitiesQuery, BaseResult<ValidateTripQuantitiesResult>>(model);

        async Task<BaseResult<long>> CreateDistributionTrip(IMediator mediator, CreateDistributionTripCommand model)
            => await mediator.Send<CreateDistributionTripCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdateDistributionTrip(IMediator mediator, UpdateDistributionTripCommand model)
            => await mediator.Send<UpdateDistributionTripCommand, BaseResult>(model);

        async Task<BaseResult> PostDistributionTrip(IMediator mediator, PostDistributionTripCommand model)
            => await mediator.Send<PostDistributionTripCommand, BaseResult>(model);

        async Task<BaseResult> ReceiveGoods(IMediator mediator, ReceiveGoodsCommand model)
          => await mediator.Send<ReceiveGoodsCommand, BaseResult>(model);

        async Task<BaseResult> DeleteDistributionTrip(IMediator mediator, long tripId)
            => await mediator.Send<DeleteDistributionTripCommand, BaseResult>(new DeleteDistributionTripCommand(tripId));
    }
}


