using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface ICashRegisterRepository : IGenericRepository<CashRegister>
	{
		Task<PaginationResponseDto<CashRegisterDto>> GetPagedListAsync(CashRegisterFilter filter);
	}
}


