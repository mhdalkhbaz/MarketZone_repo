using MarketZone.Application.Features.Logistics.Commands.Regions.CreateRegion;
using MarketZone.Application.Features.Logistics.Commands.Regions.UpdateRegion;
using MarketZone.Application.Features.Logistics.Queries.GetPagedListRegion;
using MarketZone.Application.Features.Logistics.Queries.GetRegionById;
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
    public class RegionEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedListRegion);
            builder.MapPost(CreateRegion).RequireAuthorization();
            builder.MapPut(UpdateRegion).RequireAuthorization();
            builder.MapGet(GetRegionById);
        }

        async Task<PagedResponse<RegionDto>> GetPagedListRegion(IMediator mediator, [AsParameters] GetPagedListRegionQuery model)
            => await mediator.Send<GetPagedListRegionQuery, PagedResponse<RegionDto>>(model);

        async Task<BaseResult<RegionDto>> GetRegionById(IMediator mediator, [AsParameters] GetRegionByIdQuery model)
            => await mediator.Send<GetRegionByIdQuery, BaseResult<RegionDto>>(model);

        async Task<BaseResult<long>> CreateRegion(IMediator mediator, CreateRegionCommand model)
            => await mediator.Send<CreateRegionCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdateRegion(IMediator mediator, UpdateRegionCommand model)
            => await mediator.Send<UpdateRegionCommand, BaseResult>(model);
    }
}


