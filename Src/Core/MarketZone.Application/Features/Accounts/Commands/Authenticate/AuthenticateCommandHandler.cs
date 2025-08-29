using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs.Account.Responses;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.UserInterfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Accounts.Commands.Authenticate
{
    public class AuthenticateCommandHandler(IAccountServices accountServices) : IRequestHandler<AuthenticateCommand, BaseResult<AuthenticationResponse>>
    {
        public async Task<BaseResult<AuthenticationResponse>> Handle(AuthenticateCommand request, CancellationToken cancellationToken = default)
        {
            return await accountServices.Authenticate(request);
        }
    }
}