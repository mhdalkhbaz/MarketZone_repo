using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs.Account.Responses;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.UserInterfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Accounts.Commands.Start
{
    public class StartCommandHandler(IAccountServices accountServices) : IRequestHandler<StartCommand, BaseResult<AuthenticationResponse>>
    {
        public async Task<BaseResult<AuthenticationResponse>> Handle(StartCommand request, CancellationToken cancellationToken = default)
        {
            var ghostUsername = await accountServices.RegisterGhostAccount();
            return await accountServices.AuthenticateByUserName(ghostUsername.Data);
        }
    }
}