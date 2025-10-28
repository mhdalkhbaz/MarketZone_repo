using MarketZone.Application.Features.Cash.CashRegisters.Commands.CreateCashRegister;
using MarketZone.Application.Features.Cash.CashRegisters.Commands.UpdateCashRegister;
using MarketZone.Application.Features.Cash.CashRegisters.Queries.GetPagedListCashRegister;
using MarketZone.Application.Features.Cash.CashRegisters.Queries.GetCashRegisterById;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace MarketZone.WebApi.Endpoints
{
    public class CashEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            // CashRegisters
            builder.MapGet(GetPagedListCashRegister);
            builder.MapGet(GetCashRegisterById);
            builder.MapPost(CreateCashRegister).RequireAuthorization();
            builder.MapPut(UpdateCashRegister).RequireAuthorization();
        }

        // CashRegisters
        async Task<PagedResponse<CashRegisterDto>> GetPagedListCashRegister(IMediator mediator, [AsParameters] GetPagedListCashRegisterQuery model)
            => await mediator.Send<GetPagedListCashRegisterQuery, PagedResponse<CashRegisterDto>>(model);

        async Task<BaseResult<CashRegisterDto>> GetCashRegisterById(IMediator mediator, [AsParameters] GetCashRegisterByIdQuery model)
            => await mediator.Send<GetCashRegisterByIdQuery, BaseResult<CashRegisterDto>>(model);

        async Task<BaseResult<long>> CreateCashRegister(IMediator mediator, CreateCashRegisterCommand model)
            => await mediator.Send<CreateCashRegisterCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdateCashRegister(IMediator mediator, UpdateCashRegisterCommand model)
            => await mediator.Send<UpdateCashRegisterCommand, BaseResult>(model);

    }
}


