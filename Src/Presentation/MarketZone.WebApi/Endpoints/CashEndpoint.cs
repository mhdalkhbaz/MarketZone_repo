using MarketZone.Application.Features.Cash.CashRegisters.Commands.CreateCashRegister;
using MarketZone.Application.Features.Cash.CashRegisters.Commands.UpdateCashRegister;
using MarketZone.Application.Features.Cash.CashRegisters.Queries.GetPagedListCashRegister;
using MarketZone.Application.Features.Cash.CashTransactions.Commands.CreateCashTransaction;
using MarketZone.Application.Features.Cash.CashTransactions.Queries.GetPagedListCashTransaction;
using MarketZone.Application.Features.Cash.Payments.Commands.CreatePayment;
using MarketZone.Application.Features.Cash.Payments.Commands.PostPayment;
using MarketZone.Application.Features.Cash.ExchangeRates.Commands.CreateExchangeRate;
using MarketZone.Application.Features.Cash.ExchangeRates.Queries.GetLatestRate;
using MarketZone.Application.Features.Cash.ExchangeTransactions.Commands.CreateExchangeTransaction;
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
            builder.MapPost(CreateCashRegister).RequireAuthorization();
            builder.MapPut(UpdateCashRegister).RequireAuthorization();

            // CashTransactions
            builder.MapGet(GetPagedListCashTransaction);
            builder.MapPost(CreateCashTransaction).RequireAuthorization();

            // Payments
            builder.MapPost(CreatePayment).RequireAuthorization();
            builder.MapPut(PostPayment).RequireAuthorization();

            // ExchangeRates
            builder.MapPost(CreateExchangeRate).RequireAuthorization();
            builder.MapGet(GetLatestExchangeRate);
            
            // ExchangeTransactions
            builder.MapPost(CreateExchangeTransaction).RequireAuthorization();
        }

        // CashRegisters
        async Task<PagedResponse<CashRegisterDto>> GetPagedListCashRegister(IMediator mediator, [AsParameters] GetPagedListCashRegisterQuery model)
            => await mediator.Send<GetPagedListCashRegisterQuery, PagedResponse<CashRegisterDto>>(model);

        async Task<BaseResult<long>> CreateCashRegister(IMediator mediator, CreateCashRegisterCommand model)
            => await mediator.Send<CreateCashRegisterCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdateCashRegister(IMediator mediator, UpdateCashRegisterCommand model)
            => await mediator.Send<UpdateCashRegisterCommand, BaseResult>(model);

        // CashTransactions
        async Task<PagedResponse<CashTransactionDto>> GetPagedListCashTransaction(IMediator mediator, [AsParameters] GetPagedListCashTransactionQuery model)
            => await mediator.Send<GetPagedListCashTransactionQuery, PagedResponse<CashTransactionDto>>(model);

        async Task<BaseResult<long>> CreateCashTransaction(IMediator mediator, CreateCashTransactionCommand model)
            => await mediator.Send<CreateCashTransactionCommand, BaseResult<long>>(model);

        // Payments
        async Task<BaseResult<long>> CreatePayment(IMediator mediator, CreatePaymentCommand model)
            => await mediator.Send<CreatePaymentCommand, BaseResult<long>>(model);

        async Task<BaseResult> PostPayment(IMediator mediator, PostPaymentCommand model)
            => await mediator.Send<PostPaymentCommand, BaseResult>(model);

        // ExchangeRates
        async Task<BaseResult<long>> CreateExchangeRate(IMediator mediator, CreateExchangeRateCommand model)
            => await mediator.Send<CreateExchangeRateCommand, BaseResult<long>>(model);

        async Task<BaseResult<MarketZone.Domain.Cash.Entities.ExchangeRate>> GetLatestExchangeRate(IMediator mediator, [AsParameters] GetLatestRateQuery model)
            => await mediator.Send<GetLatestRateQuery, BaseResult<MarketZone.Domain.Cash.Entities.ExchangeRate>>(model);

        // ExchangeTransactions
        async Task<BaseResult<long>> CreateExchangeTransaction(IMediator mediator, CreateExchangeTransactionCommand model)
            => await mediator.Send<CreateExchangeTransactionCommand, BaseResult<long>>(model);
    }
}


