using System.Threading.Tasks;
using MarketZone.Application.Features.Cash.Payments.Commands.CreatePayment;
using MarketZone.Application.Features.Cash.Payments.Commands.PostPayment;
using MarketZone.Application.Features.Cash.Payments.Commands.UpdatePayment;
using MarketZone.Application.Features.Cash.Payments.Queries.GetPagedListPayment;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;

namespace MarketZone.WebApi.Endpoints
{
    public class PaymentEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedListPayment);
            builder.MapPost(CreatePayment).RequireAuthorization();
            builder.MapPut(UpdatePayment).RequireAuthorization();
            builder.MapPut(PostPayment).RequireAuthorization();
        }

        async Task<PagedResponse<PaymentDto>> GetPagedListPayment(IMediator mediator, [AsParameters] GetPagedListPaymentQuery model)
            => await mediator.Send<GetPagedListPaymentQuery, PagedResponse<PaymentDto>>(model);

        async Task<BaseResult<long>> CreatePayment(IMediator mediator, CreatePaymentCommand model)
            => await mediator.Send<CreatePaymentCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdatePayment(IMediator mediator, UpdatePaymentCommand model)
            => await mediator.Send<UpdatePaymentCommand, BaseResult>(model);

        async Task<BaseResult> PostPayment(IMediator mediator, [AsParameters] PostPaymentCommand model)
            => await mediator.Send<PostPaymentCommand, BaseResult>(model);
    }
}
