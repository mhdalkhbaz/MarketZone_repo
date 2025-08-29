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
	public class CashRegisterRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<CashRegister>(dbContext), ICashRegisterRepository
	{
		public async Task<PaginationResponseDto<CashRegisterDto>> GetPagedListAsync(int pageNumber, int pageSize, string name)
		{
			var query = dbContext.Set<CashRegister>().OrderByDescending(p => p.Created).AsQueryable();
			if (!string.IsNullOrEmpty(name))
				query = query.Where(p => p.Name.Contains(name));
			return await Paged(query.ProjectTo<CashRegisterDto>(mapper.ConfigurationProvider), pageNumber, pageSize);
		}
	}
}


