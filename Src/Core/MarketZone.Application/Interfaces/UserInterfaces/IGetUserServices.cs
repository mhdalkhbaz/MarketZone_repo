using System.Threading.Tasks;
using MarketZone.Application.DTOs.Account.Requests;
using MarketZone.Application.DTOs.Account.Responses;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Interfaces.UserInterfaces
{
    public interface IGetUserServices
    {
        Task<PagedResponse<UserDto>> GetPagedUsers(GetAllUsersRequest model);
    }
}
