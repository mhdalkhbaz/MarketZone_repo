using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Products.DTOs;
using MarketZone.Domain.Products.Entities;
using MarketZone.Domain.Inventory.Entities;
using MarketZone.Domain.Products.Enums;
using MarketZone.Infrastructure.Persistence.Contexts;
using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PaginationResponseDto<ProductDto>> GetPagedListAsync(ProductFilter filter)
        {
            // Start with base query on entity level to apply filters before projection
            var baseQuery = _dbContext.Products.AsQueryable();

            // Apply filters using FilterBuilder pattern
            if (!string.IsNullOrEmpty(filter.Name))
            {
                baseQuery = baseQuery.Where(p => p.Name.Contains(filter.Name));
            }

            if (!string.IsNullOrEmpty(filter.Description))
            {
                baseQuery = baseQuery.Where(p => !string.IsNullOrEmpty(p.Description) && p.Description.Contains(filter.Description));
            }

            if (filter.Status.HasValue)
            {
                // Map status to IsActive: 1 = true, 0 = false
                baseQuery = baseQuery.Where(p => p.IsActive == (filter.Status.Value == 1));
            }

            if (filter.Type.HasValue)
            {
                // Map type to ProductType enum
                baseQuery = baseQuery.Where(p => (int)p.ProductType == filter.Type.Value);
            }

            // Now project to DTO with join
            var query = from p in baseQuery
                        join pb in _dbContext.ProductBalances on p.Id equals pb.ProductId into balanceGroup
                        from balance in balanceGroup.DefaultIfEmpty()
                        orderby p.Created descending
                        select new ProductDto
                        {
                            Id = p.Id,
                            CategoryId = p.CategoryId,
                            Name = p.Name,
                            Description = p.Description,
                            UnitOfMeasure = p.UnitOfMeasure,
							// Purchase price comes from AverageCost in ProductBalance
							PurchasePrice = balance != null ? balance.AverageCost : 0m,
							SalePrice = balance != null ? balance.SalePrice : 0m,
                            MinStockLevel = p.MinStockLevel,
                            IsActive = p.IsActive,
                            NeedsRoasting = p.NeedsRoasting,
                            RoastingCost = 0m,
                            CommissionPerKg = p.CommissionPerKg,
                            RawProductId = p.RawProductId,
                            BarCode = p.BarCode,
                            CreatedDateTime = p.Created,
                            Qty = balance != null ? balance.Qty : 0,
							AvailableQty = balance != null ? balance.AvailableQty : 0,
							AverageCost = balance != null ? balance.AverageCost : 0m,
                            ProductType = p.ProductType
                        };

            return await Paged(query, filter.PageNumber, filter.PageSize);
        }

        public async Task<Dictionary<long, Product>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
        {
            var list = await _dbContext.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync(cancellationToken);
            return list.ToDictionary(p => p.Id, p => p);
        }

        public async Task<List<ProductForCompositeDto>> GetProductsForCompositeAsync(CancellationToken cancellationToken = default)
        {
            var query = from p in _dbContext.Products
                        join pb in _dbContext.ProductBalances on p.Id equals pb.ProductId
                        where pb.Qty > 0 && p.IsActive
                        orderby p.Name
                        select new ProductForCompositeDto
                        {
                            resultingProductId = p.Id,
                            resultingProductName = p.Name,
                            Qty = pb.Qty,
                            SalePrice = pb.SalePrice,
                            CommissionPerKg = p.CommissionPerKg
                        };

            return await query.ToListAsync(cancellationToken);
        }
    }
}
