using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface ICashRegisterRepository : IGenericRepository<CashRegister>
	{
		Task<PaginationResponseDto<CashRegisterDto>> GetPagedListAsync(int pageNumber, int pageSize, string name);
	}
}


