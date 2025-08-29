using MarketZone.Application.DTOs;
using MarketZone.Application.Features.Employees.Commands.CreateEmployee;
using MarketZone.Application.Features.Employees.Commands.DeleteEmployee;
using MarketZone.Application.Features.Employees.Commands.UpdateEmployee;
using MarketZone.Application.Features.Employees.Queries.GetActiveEmployeesSelectList;
using MarketZone.Application.Features.Employees.Queries.GetEmployeeById;
using MarketZone.Application.Features.Employees.Queries.GetPagedListEmployee;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Employees.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketZone.WebApi.Endpoints
{
    public class EmployeeEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedListEmployee);
            builder.MapGet(GetEmployeeById);
            builder.MapPost(CreateEmployee).RequireAuthorization();
            builder.MapPut(UpdateEmployee).RequireAuthorization();
            builder.MapDelete(DeleteEmployee).RequireAuthorization();
            builder.MapGet(GetActiveEmployeesSelectList);
        }

        async Task<PagedResponse<EmployeeDto>> GetPagedListEmployee(IMediator mediator, [AsParameters] GetPagedListEmployeeQuery model)
            => await mediator.Send<GetPagedListEmployeeQuery, PagedResponse<EmployeeDto>>(model);

        async Task<BaseResult<EmployeeDto>> GetEmployeeById(IMediator mediator, [AsParameters] GetEmployeeByIdQuery model)
            => await mediator.Send<GetEmployeeByIdQuery, BaseResult<EmployeeDto>>(model);

        async Task<BaseResult<long>> CreateEmployee(IMediator mediator, CreateEmployeeCommand model)
            => await mediator.Send<CreateEmployeeCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdateEmployee(IMediator mediator, UpdateEmployeeCommand model)
            => await mediator.Send<UpdateEmployeeCommand, BaseResult>(model);

        async Task<BaseResult> DeleteEmployee(IMediator mediator, [AsParameters] DeleteEmployeeCommand model)
            => await mediator.Send<DeleteEmployeeCommand, BaseResult>(model);

        async Task<BaseResult<List<SelectListDto>>> GetActiveEmployeesSelectList(IMediator mediator)
            => await mediator.Send<GetActiveEmployeesSelectListQuery, BaseResult<List<SelectListDto>>>(new GetActiveEmployeesSelectListQuery());
    }
}



