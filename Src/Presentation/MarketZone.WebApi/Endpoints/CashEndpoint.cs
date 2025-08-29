using MarketZone.Application.Features.Cash.CashRegisters.Commands.CreateCashRegister;
using MarketZone.Application.Features.Cash.CashRegisters.Commands.UpdateCashRegister;
using MarketZone.Application.Features.Cash.CashRegisters.Queries.GetPagedListCashRegister;
using MarketZone.Application.Features.Cash.CashTransactions.Commands.CreateCashTransaction;
using MarketZone.Application.Features.Cash.CashTransactions.Queries.GetPagedListCashTransaction;
using MarketZone.Application.Features.Cash.Expenses.Commands.CreateExpense;
using MarketZone.Application.Features.Cash.Expenses.Commands.UpdateExpense;
using MarketZone.Application.Features.Cash.Expenses.Queries.GetPagedListExpense;
using MarketZone.Application.Features.Cash.Expenses.Queries.GetExpenseById;
using MarketZone.Application.Features.Cash.Payments.Commands.CreatePayment;
using MarketZone.Application.Features.Cash.Payments.Commands.PostPayment;
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

            // Expenses
            builder.MapGet(GetPagedListExpense);
            builder.MapGet(GetExpenseById);
            builder.MapPost(CreateExpense).RequireAuthorization();
            builder.MapPut(UpdateExpense).RequireAuthorization();
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

        // Expenses
        async Task<PagedResponse<ExpenseDto>> GetPagedListExpense(IMediator mediator, [AsParameters] GetPagedListExpenseQuery model)
            => await mediator.Send<GetPagedListExpenseQuery, PagedResponse<ExpenseDto>>(model);

        async Task<BaseResult<ExpenseDto>> GetExpenseById(IMediator mediator, [AsParameters] GetExpenseByIdQuery model)
            => await mediator.Send<GetExpenseByIdQuery, BaseResult<ExpenseDto>>(model);

        async Task<BaseResult<long>> CreateExpense(IMediator mediator, CreateExpenseCommand model)
            => await mediator.Send<CreateExpenseCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdateExpense(IMediator mediator, UpdateExpenseCommand model)
            => await mediator.Send<UpdateExpenseCommand, BaseResult>(model);
    }
}


