using MarketZone.Application.DTOs.Account.Responses;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Accounts.Commands.Start
{
    public class StartCommand : IRequest<BaseResult<AuthenticationResponse>>
    {
    }
}