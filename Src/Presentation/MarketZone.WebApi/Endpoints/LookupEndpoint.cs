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
using MarketZone.Application.Features.Products.Queries.GetProductSelectList;
using MarketZone.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Sales.Enums;
using MarketZone.Domain.Purchases.Enums;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.WebApi.Endpoints
{
    public class LookupEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetProductSelectList);
            builder.MapGet(GetProductSelectListForDistribution);
            builder.MapGet(GetCustomerSelectList);
            builder.MapGet(GetSupplierSelectList);
            builder.MapGet(GetEmployeeSelectList);
            builder.MapGet(GetRegionSelectList);
            builder.MapGet(GetCarSelectList);
            
            // New endpoints for payment system
            builder.MapGet(GetSuppliersSelectList);
            builder.MapGet(GetPurchaseInvoicesBySupplier);
            builder.MapGet(GetCustomersSelectList);
            builder.MapGet(GetSalesInvoicesByCustomer);
            builder.MapGet(GetDeliveryTripSelectList);
            builder.MapGet(GetProductReadyByRawProductSelectList);
            builder.MapGet(GetInStockProductSelectList);
            builder.MapGet(GetUnroastedProductsWithQty);
            builder.MapGet(GetUnroastedProducts);
            builder.MapGet(GetAllProductsForPurchase);
            builder.MapGet(GetRoastingEmployeesSelectList);
            builder.MapGet(GetRoastingEmployeesWithBalance);
            builder.MapGet(GetCustomersWithDebts);
            builder.MapGet(GetSuppliersWithDebts);
        }
        async Task<BaseResult<List<SelectListDto>>> GetProductSelectList(ApplicationDbContext db, IMapper mapper)
            => BaseResult<List<SelectListDto>>.Ok(await db.Products.AsNoTracking().OrderBy(p => p.Id).ProjectTo<SelectListDto>(mapper.ConfigurationProvider).ToListAsync());

        async Task<BaseResult<List<ProductSelectListDto>>> GetProductSelectListForDistribution(IMediator mediator, [AsParameters] GetProductSelectListQuery model)
            => await mediator.Send<GetProductSelectListQuery, BaseResult<List<ProductSelectListDto>>>(model);

        async Task<BaseResult<List<SelectListWithCurrencyDto>>> GetCustomerSelectList(ApplicationDbContext db, IMapper mapper)
            => BaseResult<List<SelectListWithCurrencyDto>>.Ok(await db.Customers.AsNoTracking().OrderBy(p => p.Id).ProjectTo<SelectListWithCurrencyDto>(mapper.ConfigurationProvider).ToListAsync());

        async Task<BaseResult<List<SelectListWithCurrencyDto>>> GetSupplierSelectList(ApplicationDbContext db, IMapper mapper)
            => BaseResult<List<SelectListWithCurrencyDto>>.Ok(await db.Suppliers.AsNoTracking().OrderBy(p => p.Id).ProjectTo<SelectListWithCurrencyDto>(mapper.ConfigurationProvider).ToListAsync());

        async Task<BaseResult<List<SelectListDto>>> GetEmployeeSelectList(ApplicationDbContext db, IMapper mapper)
            => BaseResult<List<SelectListDto>>.Ok(await db.Employees.AsNoTracking().OrderBy(p => p.FirstName + p.LastName).ProjectTo<SelectListDto>(mapper.ConfigurationProvider).ToListAsync());

        // Only products with AvailableQty > 0
        async Task<BaseResult<List<UnroastedProductDto>>> GetInStockProductSelectList(ApplicationDbContext db)
        {
            var query = from p in db.Products.AsNoTracking()
                        join b in db.ProductBalances.AsNoTracking() on p.Id equals b.ProductId
                        where b.AvailableQty > 0 && !b.Product.NeedsRoasting
                        orderby p.Id
                        select new UnroastedProductDto(p.Id.ToString(), p.Name, b.AvailableQty, b.AverageCost);
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
                .Where(t => t.Status == DistributionTripStatus.GoodsReceived)
                .OrderByDescending(t => t.TripDate)
                .Select(t => new { t.Id, t.TripDate })
                .ToListAsync();

            var list = trips
                .Select(t => new SelectListDto($"{t.TripDate:yyyy-MM-dd} ({t.Id})", t.Id.ToString()))
                .ToList();

            return BaseResult<List<SelectListDto>>.Ok(list);
        }

        async Task<BaseResult<List<SupplierSelectListDto>>> GetSuppliersSelectList(ApplicationDbContext db)
        {
            var suppliers = await db.Suppliers.AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .Select(s => new SupplierSelectListDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Currency = s.Currency ?? MarketZone.Domain.Cash.Enums.Currency.SY // Default currency
                })
                .ToListAsync();

            return BaseResult<List<SupplierSelectListDto>>.Ok(suppliers);
        }

        async Task<BaseResult<List<PurchaseInvoiceSelectListDto>>> GetPurchaseInvoicesBySupplier(ApplicationDbContext db, long supplierId)
        {
            var invoices = await db.PurchaseInvoices.AsNoTracking()
                .Where(p => p.SupplierId == supplierId && p.Status == PurchaseInvoiceStatus.Posted)
                .OrderByDescending(p => p.InvoiceDate)
                .Select(p => new PurchaseInvoiceSelectListDto
                {
                    Id = p.Id,
                    InvoiceNumber = p.InvoiceNumber,
                    TotalAmount = p.TotalAmount,
                    Currency = p.Currency ?? MarketZone.Domain.Cash.Enums.Currency.SY,
                    InvoiceDate = p.InvoiceDate,
                    Status = p.Status.ToString()
                })
                .ToListAsync();

            return BaseResult<List<PurchaseInvoiceSelectListDto>>.Ok(invoices);
        }

        async Task<BaseResult<List<CustomerSelectListDto>>> GetCustomersSelectList(ApplicationDbContext db)
        {
            var customers = await db.Customers.AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new CustomerSelectListDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Currency = c.Currency ?? MarketZone.Domain.Cash.Enums.Currency.SY, // Default currency
                    Phone = c.Phone,
                    Address = c.Address
                })
                .ToListAsync();

            return BaseResult<List<CustomerSelectListDto>>.Ok(customers);
        }

        async Task<BaseResult<List<SalesInvoiceSelectListDto>>> GetSalesInvoicesByCustomer(ApplicationDbContext db, long customerId)
        {
            var invoices = await db.SalesInvoices.AsNoTracking()
                .Where(s => s.CustomerId == customerId && s.Status == SalesInvoiceStatus.Posted)
                .OrderByDescending(s => s.InvoiceDate)
                .Select(s => new SalesInvoiceSelectListDto
                {
                    Id = s.Id,
                    InvoiceNumber = s.InvoiceNumber,
                    TotalAmount = s.TotalAmount,
                    Currency = s.Currency ?? MarketZone.Domain.Cash.Enums.Currency.SY,
                    InvoiceDate = s.InvoiceDate,
                    Status = s.Status.ToString(),
                    DistributionTripId = s.DistributionTripId,
                    CustomerName = s.Customer.Name
                })
                .ToListAsync();

            return BaseResult<List<SalesInvoiceSelectListDto>>.Ok(invoices);
        }

        async Task<BaseResult<List<SelectListDto>>> GetProductReadyByRawProductSelectList(ApplicationDbContext db, long productId)
        {
            var products = await db.Products.AsNoTracking()
                .Where(p => p.RawProductId == productId)
                .OrderBy(p => p.Name)
                .Select(p => new SelectListDto(p.Name, p.Id.ToString()))
                .ToListAsync();

            return BaseResult<List<SelectListDto>>.Ok(products);
        }

        async Task<BaseResult<List<UnroastedProductDto>>> GetUnroastedProducts(ApplicationDbContext db)
        {
            var query = from p in db.Products.AsNoTracking()
                        where p.NeedsRoasting == true // فقط المنتجات التي تحتاج تحميص
                        orderby p.Name
                        select new UnroastedProductDto(p.Id.ToString(), p.Name, 0);
            var list = await query.ToListAsync();
            return BaseResult<List<UnroastedProductDto>>.Ok(list);
        }
        async Task<BaseResult<List<UnroastedProductDto>>> GetAllProductsForPurchase(ApplicationDbContext db)
        {
            var query = from p in db.Products.AsNoTracking()
                        where p.RawProductId == null  
                        orderby p.Name
                        select new UnroastedProductDto(p.Id.ToString(), p.Name, 0);
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

        // Returns employees with job title "roasting" (محماصين)
        async Task<BaseResult<List<SelectListWithCurrencyDto>>> GetRoastingEmployeesSelectList(ApplicationDbContext db)
        {
            var employees = await db.Employees.AsNoTracking()
                .Where(e => e.JobTitle == "roasting" && e.IsActive)
                .OrderBy(e => e.FirstName + " " + e.LastName)
                .Select(e => new SelectListWithCurrencyDto(
                    e.FirstName + " " + e.LastName,
                    e.Id.ToString(),
                    e.Currency ?? Currency.SY
                ))
                .ToListAsync();

            return BaseResult<List<SelectListWithCurrencyDto>>.Ok(employees);
        }

        // Returns roasting employees who have balance (SyrianMoney or DollarMoney > 0)
        async Task<BaseResult<List<SelectListDto>>> GetRoastingEmployeesWithBalance(ApplicationDbContext db)
        {
            var employees = await db.Employees.AsNoTracking()
                .Where(e => e.JobTitle == "roasting" && e.IsActive && 
                           (e.SyrianMoney > 0 || e.DollarMoney > 0))
                .OrderBy(e => e.FirstName + " " + e.LastName)
                .Select(e => new SelectListDto(e.FirstName + " " + e.LastName, e.Id.ToString()))
                .ToListAsync();

            return BaseResult<List<SelectListDto>>.Ok(employees);
        }

		// Returns customers who have invoices with incomplete payment status (customers with debts)
		async Task<BaseResult<List<SelectListWithCurrencyDto>>> GetCustomersWithDebts(ApplicationDbContext db)
		{
			var customersWithDebts = await db.SalesInvoices
				.Where(si => si.Status == SalesInvoiceStatus.Posted)
				.GroupJoin(
					db.Payments.Where(p => p.PaymentType == PaymentType.SalesPayment && p.Status == MarketZone.Domain.Cash.Entities.PaymentStatus.Posted),
					si => si.Id,
					p => p.InvoiceId,
					(si, payments) => new { Invoice = si, Payments = payments }
				)
				.SelectMany(
					x => x.Payments.DefaultIfEmpty(),
					(x, payment) => new { x.Invoice, Payment = payment }
				)
				.GroupBy(x => x.Invoice.Id)
				.Select(g => new
				{
					InvoiceId = g.Key,
					CustomerId = g.First().Invoice.CustomerId,
					CustomerName = g.First().Invoice.Customer.Name,
					CustomerCurrency = g.First().Invoice.Customer.Currency ?? Currency.SY,
					TotalAmount = g.First().Invoice.TotalAmount - g.First().Invoice.Discount,
					PaidAmount = g.Where(x => x.Payment != null).Sum(x => x.Payment.AmountInPaymentCurrency ?? x.Payment.Amount)
				})
				.Where(x => x.PaidAmount < x.TotalAmount) // فواتير لم يتم دفعها بالكامل (جزئياً أو غير مسددة)
				.GroupBy(x => x.CustomerId)
				.Select(g => new
				{
					CustomerId = g.Key,
					CustomerName = g.First().CustomerName,
					CustomerCurrency = g.First().CustomerCurrency
				})
				.OrderBy(x => x.CustomerName)
				.Select(x => new SelectListWithCurrencyDto(x.CustomerName, x.CustomerId.ToString(), x.CustomerCurrency))
				.ToListAsync();

			return BaseResult<List<SelectListWithCurrencyDto>>.Ok(customersWithDebts);
		}

        // Returns suppliers who have invoices with incomplete payment status (suppliers with debts)
        async Task<BaseResult<List<SelectListDto>>> GetSuppliersWithDebts(ApplicationDbContext db)
        {
            var suppliersWithDebts = await db.PurchaseInvoices
                .Where(pi => pi.Status == MarketZone.Domain.Purchases.Enums.PurchaseInvoiceStatus.Posted)
                .GroupJoin(
                    db.Payments.Where(p => p.PaymentType == PaymentType.PurchasePayment && p.Status == MarketZone.Domain.Cash.Entities.PaymentStatus.Posted),
                    pi => pi.Id,
                    p => p.InvoiceId,
                    (pi, payments) => new { Invoice = pi, Payments = payments }
                )
                .SelectMany(
                    x => x.Payments.DefaultIfEmpty(),
                    (x, payment) => new { x.Invoice, Payment = payment }
                )
                .GroupBy(x => x.Invoice.Id)
                .Select(g => new
                {
                    InvoiceId = g.Key,
                    SupplierId = g.First().Invoice.SupplierId,
                    SupplierName = g.First().Invoice.Supplier.Name,
                    TotalAmount = g.First().Invoice.TotalAmount - g.First().Invoice.Discount,
                    PaidAmount = g.Where(x => x.Payment != null).Sum(x => x.Payment.AmountInPaymentCurrency ?? x.Payment.Amount)
                })
                .Where(x => x.PaidAmount < x.TotalAmount) // فواتير لم يتم دفعها بالكامل (جزئياً أو غير مسددة)
                .GroupBy(x => x.SupplierId)
                .Select(g => new
                {
                    SupplierId = g.Key,
                    SupplierName = g.First().SupplierName
                })
                .OrderBy(x => x.SupplierName)
                .Select(x => new SelectListDto(x.SupplierName, x.SupplierId.ToString()))
                .ToListAsync();

            return BaseResult<List<SelectListDto>>.Ok(suppliersWithDebts);
        }

    }
}


