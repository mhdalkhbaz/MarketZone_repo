using MarketZone.Application.DTOs;
using MarketZone.Application.Features.Suppliers.Commands.CreateSupplier;
using MarketZone.Application.Features.Suppliers.Commands.DeleteSupplier;
using MarketZone.Application.Features.Suppliers.Commands.UpdateSupplier;
using MarketZone.Application.Features.Suppliers.Queries.GetActiveSuppliersSelectList;
using MarketZone.Application.Features.Suppliers.Queries.GetPagedListSupplier;
using MarketZone.Application.Features.Suppliers.Queries.GetSupplierById;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Suppliers.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketZone.WebApi.Endpoints
{
    public class SupplierEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedListSupplier);
            builder.MapGet(GetSupplierById);
            builder.MapPost(CreateSupplier).RequireAuthorization();
            builder.MapPut(UpdateSupplier).RequireAuthorization();
            builder.MapDelete(DeleteSupplier).RequireAuthorization();
            builder.MapGet(GetActiveSuppliersSelectList);
        }

        async Task<PagedResponse<SupplierDto>> GetPagedListSupplier(IMediator mediator, [AsParameters] GetPagedListSupplierQuery model)
            => await mediator.Send<GetPagedListSupplierQuery, PagedResponse<SupplierDto>>(model);

        async Task<BaseResult<SupplierDto>> GetSupplierById(IMediator mediator, [AsParameters] GetSupplierByIdQuery model)
            => await mediator.Send<GetSupplierByIdQuery, BaseResult<SupplierDto>>(model);

        async Task<BaseResult<long>> CreateSupplier(IMediator mediator, [FromBody] CreateSupplierCommand model)
            => await mediator.Send<CreateSupplierCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdateSupplier(IMediator mediator, [FromBody] UpdateSupplierCommand model)
            => await mediator.Send<UpdateSupplierCommand, BaseResult>(model);

        async Task<BaseResult> DeleteSupplier(IMediator mediator, [AsParameters] DeleteSupplierCommand model)
            => await mediator.Send<DeleteSupplierCommand, BaseResult>(model);

        async Task<BaseResult<List<SelectListDto>>> GetActiveSuppliersSelectList(IMediator mediator)
            => await mediator.Send<GetActiveSuppliersSelectListQuery, BaseResult<List<SelectListDto>>>(new GetActiveSuppliersSelectListQuery());
    }
}



