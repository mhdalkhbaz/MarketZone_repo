using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface IExpenseRepository : IGenericRepository<Expense>
	{
		Task<PaginationResponseDto<ExpenseDto>> GetPagedListAsync(int pageNumber, int pageSize, string description);
	}
}


