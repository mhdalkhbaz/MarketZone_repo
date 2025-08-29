using MarketZone.Application.DTOs;
using MarketZone.Application.Features.Customers.Commands.CreateCustomer;
using MarketZone.Application.Features.Customers.Commands.DeleteCustomer;
using MarketZone.Application.Features.Customers.Commands.UpdateCustomer;
using MarketZone.Application.Features.Customers.Queries.GetActiveCustomersSelectList;
using MarketZone.Application.Features.Customers.Queries.GetCustomerById;
using MarketZone.Application.Features.Customers.Queries.GetPagedListCustomer;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Customers.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketZone.WebApi.Endpoints
{
    public class CustomerEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedListCustomer);
            builder.MapGet(GetCustomerById);
            builder.MapPost(CreateCustomer).RequireAuthorization();
            builder.MapPut(UpdateCustomer).RequireAuthorization();
            builder.MapDelete(DeleteCustomer).RequireAuthorization();
            builder.MapGet(GetActiveCustomersSelectList);
        }

        async Task<PagedResponse<CustomerDto>> GetPagedListCustomer(IMediator mediator, [AsParameters] GetPagedListCustomerQuery model)
            => await mediator.Send<GetPagedListCustomerQuery, PagedResponse<CustomerDto>>(model);

        async Task<BaseResult<CustomerDto>> GetCustomerById(IMediator mediator, [AsParameters] GetCustomerByIdQuery model)
            => await mediator.Send<GetCustomerByIdQuery, BaseResult<CustomerDto>>(model);

        async Task<BaseResult<long>> CreateCustomer(IMediator mediator, CreateCustomerCommand model)
            => await mediator.Send<CreateCustomerCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdateCustomer(IMediator mediator, UpdateCustomerCommand model)
            => await mediator.Send<UpdateCustomerCommand, BaseResult>(model);

        async Task<BaseResult> DeleteCustomer(IMediator mediator, [AsParameters] DeleteCustomerCommand model)
            => await mediator.Send<DeleteCustomerCommand, BaseResult>(model);

        async Task<BaseResult<List<SelectListDto>>> GetActiveCustomersSelectList(IMediator mediator)
            => await mediator.Send<GetActiveCustomersSelectListQuery, BaseResult<List<SelectListDto>>>(new GetActiveCustomersSelectListQuery());
    }
}



