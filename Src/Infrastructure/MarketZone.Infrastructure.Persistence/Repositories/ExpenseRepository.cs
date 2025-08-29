using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class ExpenseRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<Expense>(dbContext), IExpenseRepository
	{
		public async Task<PaginationResponseDto<ExpenseDto>> GetPagedListAsync(int pageNumber, int pageSize, string description)
		{
			var query = dbContext.Set<Expense>().OrderByDescending(p => p.ExpenseDate).AsQueryable();
			
			if (!string.IsNullOrEmpty(description))
			{
				query = query.Where(p => p.Description.Contains(description));
			}
			
			return await Paged(query.ProjectTo<ExpenseDto>(mapper.ConfigurationProvider), pageNumber, pageSize);
		}
	}
}


