using System.Collections.Generic;
using System.Threading.Tasks;
using MarketZone.Application.Features.Roasting.Commands.CreateRoastingInvoice;
using MarketZone.Application.Features.Roasting.Commands.UpdateRoastingInvoice;
using MarketZone.Application.Features.Roasting.Commands.DeleteRoastingInvoice;
using MarketZone.Application.Features.Roasting.Commands.PostRoastingInvoice;
using MarketZone.Application.Features.Roasting.Queries.GetPagedListRoastingInvoice;
using MarketZone.Application.Features.Roasting.Queries.GetRoastingInvoiceById;
using MarketZone.Application.Features.Roasting.Queries.GetUnpaidRoastingInvoicesByEmployee;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Roasting.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MarketZone.WebApi.Endpoints
{
    public class RoastingInvoiceEndpoint : EndpointGroupBase
    {
        
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedList);
            builder.MapGet(GetById);
            builder.MapGet(GetUnpaidInvoicesByEmployee);
            builder.MapPost(Create).RequireAuthorization();
            builder.MapPut(Update).RequireAuthorization();
            builder.MapDelete(Delete).RequireAuthorization();
            builder.MapPost(Post).RequireAuthorization();
        }

        async Task<PagedResponse<RoastingInvoiceDto>> GetPagedList(IMediator mediator, [AsParameters] GetPagedListRoastingInvoiceQuery model)
            => await mediator.Send<GetPagedListRoastingInvoiceQuery, PagedResponse<RoastingInvoiceDto>>(model);

        async Task<BaseResult<RoastingInvoiceDto>> GetById(IMediator mediator, [AsParameters] GetRoastingInvoiceByIdQuery model)
            => await mediator.Send<GetRoastingInvoiceByIdQuery, BaseResult<RoastingInvoiceDto>>(model);

        async Task<BaseResult<List<RoastingInvoiceDto>>> GetUnpaidInvoicesByEmployee(IMediator mediator, [AsParameters] GetUnpaidRoastingInvoicesByEmployeeQuery model)
            => await mediator.Send<GetUnpaidRoastingInvoicesByEmployeeQuery, BaseResult<List<RoastingInvoiceDto>>>(model);

        async Task<BaseResult<long>> Create(IMediator mediator, [FromBody] CreateRoastingInvoiceCommand model)
            => await mediator.Send<CreateRoastingInvoiceCommand, BaseResult<long>>(model);

        async Task<BaseResult<long>> Update(IMediator mediator, [FromBody] UpdateRoastingInvoiceCommand model)
            => await mediator.Send<UpdateRoastingInvoiceCommand, BaseResult<long>>(model);

        async Task<BaseResult> Delete(IMediator mediator, [AsParameters] DeleteRoastingInvoiceCommand model)
            => await mediator.Send<DeleteRoastingInvoiceCommand, BaseResult>(model);

        async Task<BaseResult<long>> Post(IMediator mediator, [FromBody] PostRoastingInvoiceCommand model)
            => await mediator.Send<PostRoastingInvoiceCommand, BaseResult<long>>(model);
    }
}
