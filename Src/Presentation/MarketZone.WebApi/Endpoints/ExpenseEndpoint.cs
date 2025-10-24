using System.Threading.Tasks;
using MarketZone.Application.Features.Cash.Expenses.Commands.CreateExpense;
using MarketZone.Application.Features.Cash.Expenses.Commands.UpdateExpense;
using MarketZone.Application.Features.Cash.Expenses.Commands.DeleteExpense;
using MarketZone.Application.Features.Cash.Expenses.Queries.GetPagedListExpense;
using MarketZone.Application.Features.Cash.Expenses.Queries.GetExpenseById;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MarketZone.WebApi.Endpoints
{
    public class ExpenseEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedListExpense);
            builder.MapGet(GetExpenseById);
            builder.MapPost(CreateExpense).RequireAuthorization();
            builder.MapPut(UpdateExpense).RequireAuthorization();
            builder.MapDelete(DeleteExpense).RequireAuthorization();
        }

        async Task<PagedResponse<CashTransactionDto>> GetPagedListExpense(IMediator mediator, [AsParameters] GetPagedListExpenseQuery model)
            => await mediator.Send<GetPagedListExpenseQuery, PagedResponse<CashTransactionDto>>(model);

        async Task<BaseResult<CashTransactionDto>> GetExpenseById(IMediator mediator, [AsParameters] GetExpenseByIdQuery model)
            => await mediator.Send<GetExpenseByIdQuery, BaseResult<CashTransactionDto>>(model);

        async Task<BaseResult<long>> CreateExpense(IMediator mediator, CreateExpenseCommand model)
            => await mediator.Send<CreateExpenseCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdateExpense(IMediator mediator, UpdateExpenseCommand model)
            => await mediator.Send<UpdateExpenseCommand, BaseResult>(model);

        async Task<BaseResult> DeleteExpense(IMediator mediator, [AsParameters] DeleteExpenseCommand model)
            => await mediator.Send<DeleteExpenseCommand, BaseResult>(model);
    }
}
