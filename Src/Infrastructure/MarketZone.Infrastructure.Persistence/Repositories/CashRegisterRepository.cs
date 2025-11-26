using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Parameters;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class CashRegisterRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<CashRegister>(dbContext), ICashRegisterRepository
	{
		public async Task<PaginationResponseDto<CashRegisterDto>> GetPagedListAsync(CashRegisterFilter filter)
		{
			var query = dbContext.Set<CashRegister>().AsQueryable();

			// Apply filters using FilterBuilder pattern
			if (!string.IsNullOrEmpty(filter.Name))
			{
				query = query.Where(p => p.Name.Contains(filter.Name));
			}

			query = query.OrderByDescending(p => p.Created);

			return await Paged(query.ProjectTo<CashRegisterDto>(mapper.ConfigurationProvider), filter.PageNumber, filter.PageSize);
		}
	}
}


