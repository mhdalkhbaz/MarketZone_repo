using MarketZone.Application.DTOs.Account.Responses;
using MarketZone.Application.Features.Accounts.Commands.Authenticate;
using MarketZone.Application.Features.Accounts.Commands.ChangePassword;
using MarketZone.Application.Features.Accounts.Commands.ChangeUserName;
using MarketZone.Application.Features.Accounts.Commands.Start;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace MarketZone.WebApi.Endpoints
{
    public class AccountEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapPost(Authenticate);

            //builder.MapPut(ChangeUserName).RequireAuthorization();

            //builder.MapPut(ChangePassword).RequireAuthorization();

            //builder.MapPost(Start);
        }

        async Task<BaseResult<AuthenticationResponse>> Authenticate(IMediator mediator, AuthenticateCommand model)
            => await mediator.Send<AuthenticateCommand, BaseResult<AuthenticationResponse>>(model);

        async Task<BaseResult> ChangeUserName(IMediator mediator, ChangeUserNameCommand model)
            => await mediator.Send<ChangeUserNameCommand, BaseResult>(model);

        async Task<BaseResult> ChangePassword(IMediator mediator, ChangePasswordCommand model)

            => await mediator.Send<ChangePasswordCommand, BaseResult>(model);

        async Task<BaseResult<AuthenticationResponse>> Start(IMediator mediator)
            => await mediator.Send<StartCommand, BaseResult<AuthenticationResponse>>(new StartCommand());

    }
}
