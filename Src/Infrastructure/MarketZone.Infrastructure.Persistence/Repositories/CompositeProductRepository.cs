using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Parameters;
using MarketZone.Domain.Products.DTOs;
using MarketZone.Domain.Products.Entities;
using MarketZone.Domain.Products.Enums;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class CompositeProductRepository : GenericRepository<CompositeProduct>, ICompositeProductRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CompositeProductRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<CompositeProduct> GetWithDetailsByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<CompositeProduct>()
                .Include(x => x.Details)
                    .ThenInclude(d => d.ComponentProduct)
                .Include(x => x.ResultingProduct)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<PaginationResponseDto<CompositeProductDto>> GetPagedListAsync(CompositeProductFilter filter)
        {
            var baseQuery = _dbContext.Set<CompositeProduct>()
                .Include(x => x.ResultingProduct)
                .Include(x => x.Details)
                    .ThenInclude(d => d.ComponentProduct)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.ResultingProductName))
            {
                baseQuery = baseQuery.Where(cp => cp.ResultingProduct != null && 
                    cp.ResultingProduct.Name.Contains(filter.ResultingProductName));
            }

            if (filter.Status.HasValue)
            {
                // Map status to CompositeProductStatus: 0 = Draft, 1 = Posted
                var status = (CompositeProductStatus)filter.Status.Value;
                baseQuery = baseQuery.Where(cp => cp.Status == status);
            }

            if (filter.ResultingProductId.HasValue)
            {
                baseQuery = baseQuery.Where(cp => cp.ResultingProductId == filter.ResultingProductId.Value);
            }

            // Order by creation date descending
            baseQuery = baseQuery.OrderByDescending(cp => cp.Created);

            // Project to DTO
            var query = baseQuery.ProjectTo<CompositeProductDto>(_mapper.ConfigurationProvider);

            return await Paged(query, filter.PageNumber, filter.PageSize);
        }
    }
}

