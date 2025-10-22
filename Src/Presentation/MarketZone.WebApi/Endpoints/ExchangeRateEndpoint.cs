using System.Threading.Tasks;
using MarketZone.Application.Features.Cash.ExchangeRates.Commands.CreateExchangeRate;
using MarketZone.Application.Features.Cash.ExchangeRates.Queries.GetPagedListExchangeRate;
using MarketZone.Application.Features.Cash.ExchangeRates.Queries.GetLatestRate;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MarketZone.WebApi.Endpoints
{
    public class ExchangeRateEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedListExchangeRate);
            builder.MapGet(GetLatestRate);
            builder.MapPost(CreateExchangeRate).RequireAuthorization();
        }

        async Task<PagedResponse<ExchangeRateDto>> GetPagedListExchangeRate(IMediator mediator, [AsParameters] GetPagedListExchangeRateQuery model)
            => await mediator.Send<GetPagedListExchangeRateQuery, PagedResponse<ExchangeRateDto>>(model);

        async Task<BaseResult<ExchangeRate>> GetLatestRate(IMediator mediator)
            => await mediator.Send<GetLatestRateQuery, BaseResult<ExchangeRate>>(new GetLatestRateQuery());

        async Task<BaseResult<long>> CreateExchangeRate(IMediator mediator, CreateExchangeRateCommand model)
            => await mediator.Send<CreateExchangeRateCommand, BaseResult<long>>(model);
    }
}
