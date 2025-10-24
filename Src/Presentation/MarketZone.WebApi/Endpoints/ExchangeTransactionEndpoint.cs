using System.Threading.Tasks;
using MarketZone.Application.Features.Cash.ExchangeTransactions.Commands.CreateExchangeTransaction;
using MarketZone.Application.Features.Cash.ExchangeTransactions.Queries.GetPagedListExchangeTransaction;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MarketZone.WebApi.Endpoints
{
    public class ExchangeTransactionEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedListExchangeTransaction);
            builder.MapPost(CreateExchangeTransaction).RequireAuthorization();
        }

        async Task<PagedResponse<ExchangeTransactionDto>> GetPagedListExchangeTransaction(IMediator mediator, [AsParameters] GetPagedListExchangeTransactionQuery model)
            => await mediator.Send<GetPagedListExchangeTransactionQuery, PagedResponse<ExchangeTransactionDto>>(model);

        async Task<BaseResult<long>> CreateExchangeTransaction(IMediator mediator, CreateExchangeTransactionCommand model)
            => await mediator.Send<CreateExchangeTransactionCommand, BaseResult<long>>(model);
    }
}

