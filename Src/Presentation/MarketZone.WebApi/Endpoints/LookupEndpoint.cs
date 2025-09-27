using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Enums;
using MarketZone.Infrastructure.Persistence.Contexts;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketZone.WebApi.Endpoints
{
    public class LookupEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetProductSelectList);
            builder.MapGet(GetCustomerSelectList);
            builder.MapGet(GetSupplierSelectList);
            builder.MapGet(GetEmployeeSelectList);
            builder.MapGet(GetRegionSelectList);
            builder.MapGet(GetCarSelectList);
            builder.MapGet(GetDeliveryTripSelectList);
            builder.MapGet(GetInStockProductSelectList);
            builder.MapGet(GetUnroastedProductsWithQty);
            builder.MapGet(GetUnroastedProducts);
        }

        async Task<BaseResult<List<SelectListDto>>> GetProductSelectList(ApplicationDbContext db, IMapper mapper)
            => BaseResult<List<SelectListDto>>.Ok(await db.Products.AsNoTracking().OrderBy(p => p.Id).ProjectTo<SelectListDto>(mapper.ConfigurationProvider).ToListAsync());

        async Task<BaseResult<List<SelectListDto>>> GetCustomerSelectList(ApplicationDbContext db, IMapper mapper)
            => BaseResult<List<SelectListDto>>.Ok(await db.Customers.AsNoTracking().OrderBy(p => p.Id).ProjectTo<SelectListDto>(mapper.ConfigurationProvider).ToListAsync());

        async Task<BaseResult<List<SelectListDto>>> GetSupplierSelectList(ApplicationDbContext db, IMapper mapper)
            => BaseResult<List<SelectListDto>>.Ok(await db.Suppliers.AsNoTracking().OrderBy(p => p.Id).ProjectTo<SelectListDto>(mapper.ConfigurationProvider).ToListAsync());

        async Task<BaseResult<List<SelectListDto>>> GetEmployeeSelectList(ApplicationDbContext db, IMapper mapper)
            => BaseResult<List<SelectListDto>>.Ok(await db.Employees.AsNoTracking().OrderBy(p => p.FirstName + p.LastName).ProjectTo<SelectListDto>(mapper.ConfigurationProvider).ToListAsync());

        // Only products with AvailableQty > 0
        async Task<BaseResult<List<UnroastedProductDto>>> GetInStockProductSelectList(ApplicationDbContext db)
        {
            var query = from p in db.Products.AsNoTracking()
                        join b in db.ProductBalances.AsNoTracking() on p.Id equals b.ProductId
                        where b.AvailableQty > 0 && !b.Product.NeedsRoasting
                        orderby p.Id
                        select new UnroastedProductDto(p.Id.ToString(), p.Name, b.AvailableQty);
            var list = await query.ToListAsync();
            return BaseResult<List<UnroastedProductDto>>.Ok(list);
        }

        // Regions (optionally only active)
        async Task<BaseResult<List<SelectListDto>>> GetRegionSelectList(ApplicationDbContext db, bool? onlyActive)
        {
            var regions = db.Regions.AsNoTracking().AsQueryable();
            if (onlyActive == true)
            {
                regions = regions.Where(r => r.IsActive);
            }
            var list = await regions
                .OrderBy(r => r.Name)
                .Select(r => new SelectListDto(r.Name, r.Id.ToString()))
                .ToListAsync();
            return BaseResult<List<SelectListDto>>.Ok(list);
        }

        // Cars (optionally only available)
        async Task<BaseResult<List<SelectListDto>>> GetCarSelectList(ApplicationDbContext db, bool? onlyAvailable)
        {
            var cars = db.Cars.AsNoTracking().AsQueryable();
            if (onlyAvailable == true)
            {
                cars = cars.Where(c => c.IsAvailable);
            }
            var list = await cars
                .OrderBy(c => c.Name)
                .Select(c => new SelectListDto(c.Name, c.Id.ToString()))
                .ToListAsync();
            return BaseResult<List<SelectListDto>>.Ok(list);
        }

        async Task<BaseResult<List<SelectListDto>>> GetDeliveryTripSelectList(ApplicationDbContext db)
        {
            var trips = await db.DistributionTrips.AsNoTracking()
                .Where(t => t.Status == DistributionTripStatus.Posted)
                .OrderByDescending(t => t.TripDate)
                .Select(t => new { t.Id, t.TripDate })
                .ToListAsync();

            var list = trips
                .Select(t => new SelectListDto($"{t.TripDate:yyyy-MM-dd} ({t.Id})", t.Id.ToString()))
                .ToList();

            return BaseResult<List<SelectListDto>>.Ok(list);
        }

        async Task<BaseResult<List<UnroastedProductDto>>> GetUnroastedProducts(ApplicationDbContext db)
        {
            var query = from p in db.Products.AsNoTracking()
                        where p.NeedsRoasting == true // فقط المنتجات التي تحتاج تحميص
                        orderby p.Name
                        select new UnroastedProductDto(p.Id.ToString(), p.Name,0);
            var list = await query.ToListAsync();
            return BaseResult<List<UnroastedProductDto>>.Ok(list);
        }

        // Returns products with unroasted quantities (>0)
        async Task<BaseResult<List<UnroastedProductDto>>> GetUnroastedProductsWithQty(ApplicationDbContext db)
        {
            var query = from b in db.ProductBalances.AsNoTracking()
                        where b.AvailableQty > 0
                        join p in db.Products.AsNoTracking() on b.ProductId equals p.Id
                        where p.NeedsRoasting == true // فقط المنتجات التي تحتاج تحميص
                        orderby p.Name
                        select new UnroastedProductDto(p.Id.ToString(), p.Name, b.AvailableQty);
            var list = await query.ToListAsync();
            return BaseResult<List<UnroastedProductDto>>.Ok(list);
        }


    }
}


