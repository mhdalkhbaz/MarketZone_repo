using System.Threading.Tasks;
using MarketZone.Application.Features.Roasting.Commands.CreateRoastingInvoice;
using MarketZone.Application.Features.Roasting.Commands.UpdateRoastingInvoice;
using MarketZone.Application.Features.Roasting.Commands.DeleteRoastingInvoice;
using MarketZone.Application.Features.Roasting.Commands.PostRoastingInvoice;
using MarketZone.Application.Features.Roasting.Queries.GetPagedListRoastingInvoice;
using MarketZone.Application.Features.Roasting.Queries.GetRoastingInvoiceById;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Roasting.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MarketZone.WebApi.Endpoints
{
    public class RoastingInvoiceEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedList);
            builder.MapGet(GetById);
            builder.MapPost(Create).RequireAuthorization();
            builder.MapPut(Update).RequireAuthorization();
            builder.MapDelete(Delete).RequireAuthorization();
            builder.MapPost(Post).RequireAuthorization();
        }

        async Task<PagedResponse<RoastingInvoiceDto>> GetPagedList(IMediator mediator, [AsParameters] GetPagedListRoastingInvoiceQuery model)
            => await mediator.Send<GetPagedListRoastingInvoiceQuery, PagedResponse<RoastingInvoiceDto>>(model);

        async Task<BaseResult<RoastingInvoiceDto>> GetById(IMediator mediator, [AsParameters] GetRoastingInvoiceByIdQuery model)
            => await mediator.Send<GetRoastingInvoiceByIdQuery, BaseResult<RoastingInvoiceDto>>(model);

        async Task<BaseResult<long>> Create(IMediator mediator, CreateRoastingInvoiceCommand model)
            => await mediator.Send<CreateRoastingInvoiceCommand, BaseResult<long>>(model);

        async Task<BaseResult<long>> Update(IMediator mediator, UpdateRoastingInvoiceCommand model)
            => await mediator.Send<UpdateRoastingInvoiceCommand, BaseResult<long>>(model);

        async Task<BaseResult> Delete(IMediator mediator, DeleteRoastingInvoiceCommand model)
            => await mediator.Send<DeleteRoastingInvoiceCommand, BaseResult>(model);

        async Task<BaseResult<long>> Post(IMediator mediator, PostRoastingInvoiceCommand model)
            => await mediator.Send<PostRoastingInvoiceCommand, BaseResult<long>>(model);
    }
}
