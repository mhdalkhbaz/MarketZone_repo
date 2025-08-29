using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.UserInterfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Accounts.Commands.ChangeUserName
{
    public class ChangeUserNameCommandHandler(IAccountServices accountServices) : IRequestHandler<ChangeUserNameCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(ChangeUserNameCommand request, CancellationToken cancellationToken = default)
        {
            return await accountServices.ChangeUserName(request);
        }
    }
}