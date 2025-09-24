using MarketZone.Application.Features.Logistics.Commands.CreateCar;
using MarketZone.Application.Features.Logistics.Commands.DeleteCar;
using MarketZone.Application.Features.Logistics.Commands.UpdateCar;
using MarketZone.Application.Features.Logistics.Queries.GetPagedListCar;
using MarketZone.Application.Features.Logistics.Queries.GetCarById;
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
    public class CarEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedListCar);
            builder.MapPost(CreateCar).RequireAuthorization();
            builder.MapPut(UpdateCar).RequireAuthorization();
            builder.MapDelete(DeleteCar).RequireAuthorization();
            builder.MapGet(GetCarById);
        }

        async Task<PagedResponse<CarDto>> GetPagedListCar(IMediator mediator, [AsParameters] GetPagedListCarQuery model)
            => await mediator.Send<GetPagedListCarQuery, PagedResponse<CarDto>>(model);

        async Task<BaseResult<CarDto>> GetCarById(IMediator mediator, [AsParameters] GetCarByIdQuery model)
            => await mediator.Send<GetCarByIdQuery, BaseResult<CarDto>>(model);

        async Task<BaseResult<long>> CreateCar(IMediator mediator, CreateCarCommand model)
            => await mediator.Send<CreateCarCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdateCar(IMediator mediator, UpdateCarCommand model)
            => await mediator.Send<UpdateCarCommand, BaseResult>(model);

        async Task<BaseResult> DeleteCar(IMediator mediator, [AsParameters] DeleteCarCommand model)
            => await mediator.Send<DeleteCarCommand, BaseResult>(model);
    }
}


