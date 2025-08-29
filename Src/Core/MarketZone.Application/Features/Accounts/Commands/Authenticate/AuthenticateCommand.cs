using MarketZone.Application.DTOs.Account.Requests;
using MarketZone.Application.DTOs.Account.Responses;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Accounts.Commands.Authenticate
{
    public class AuthenticateCommand : AuthenticationRequest, IRequest<BaseResult<AuthenticationResponse>>
    {
    }
}