using MarketZone.Application.DTOs.Account.Requests;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Accounts.Commands.ChangeUserName
{
    public class ChangeUserNameCommand : ChangeUserNameRequest, IRequest<BaseResult>
    {
    }
}