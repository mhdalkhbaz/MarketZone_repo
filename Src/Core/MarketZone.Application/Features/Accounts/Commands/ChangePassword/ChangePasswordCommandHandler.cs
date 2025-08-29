using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.UserInterfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Accounts.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler(IAccountServices accountServices) : IRequestHandler<ChangePasswordCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken = default)
        {
            return await accountServices.ChangePassword(request);
        }
    }
}