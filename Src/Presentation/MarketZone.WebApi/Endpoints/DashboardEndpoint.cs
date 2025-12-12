using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MarketZone.Infrastructure.Persistence.Contexts;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Purchases.Enums;
using MarketZone.Domain.Sales.Enums;
using MarketZone.WebApi.Infrastructure.Extensions;

namespace MarketZone.WebApi.Endpoints
{
    public class DashboardEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetDashboardData);
        }

        // API واحد لإرجاع بيانات الـ Dashboard حسب القسم المطلوب
        async Task<BaseResult<DashboardDataDto>> GetDashboardData(ApplicationDbContext db, int section = 1)
        {
            var result = new DashboardDataDto();

            switch (section)
            {
                case 1: // القسم الأول: الصندوق والسعر والعدادات
                    result.CashBalance = await GetCashBalanceData(db);
                    result.DollarRate = await GetDollarRateData(db);
                    result.CustomersCount = await db.Customers.Where(c => c.IsActive).CountAsync();
                    result.SuppliersCount = await db.Suppliers.Where(s => s.IsActive).CountAsync();
                    break;

                case 2: // القسم الثاني: الديون والفواتير غير المسددة
                    result.TotalDebts = await GetTotalDebtsData(db);
                    result.TopUnpaidSalesInvoices = await GetTopUnpaidSalesInvoicesData(db);
                    result.TopUnpaidPurchaseInvoices = await GetTopUnpaidPurchaseInvoicesData(db);
                    break;

                default: // جميع البيانات
                    result.CashBalance = await GetCashBalanceData(db);
                    result.DollarRate = await GetDollarRateData(db);
                    result.CustomersCount = await db.Customers.Where(c => c.IsActive).CountAsync();
                    result.SuppliersCount = await db.Suppliers.Where(s => s.IsActive).CountAsync();
                    result.TotalDebts = await GetTotalDebtsData(db);
                    result.TopUnpaidSalesInvoices = await GetTopUnpaidSalesInvoicesData(db);
                    result.TopUnpaidPurchaseInvoices = await GetTopUnpaidPurchaseInvoicesData(db);
                    break;
            }

            return BaseResult<DashboardDataDto>.Ok(result);
        }

        // Helper methods
        private async Task<CashBalanceDto> GetCashBalanceData(ApplicationDbContext db)
        {
            var totalSyrianBalance = await db.CashRegisters.SumAsync(cr => cr.CurrentBalance);
            var totalDollarBalance = await db.CashRegisters.SumAsync(cr => cr.CurrentBalanceDollar);

            return new CashBalanceDto
            {
                TotalSyrianBalance = totalSyrianBalance,
                TotalDollarBalance = totalDollarBalance
            };
        }

        private async Task<DollarRateDto> GetDollarRateData(ApplicationDbContext db)
        {
            var latestExchange = await db.ExchangeRates
                .OrderByDescending(et => et.Created)
                .FirstOrDefaultAsync();

            if (latestExchange == null)
            {
                return new DollarRateDto { Rate = 0m, LastUpdated = DateTime.MinValue };
            }

            return new DollarRateDto
            {
                Rate = latestExchange.Rate,
                LastUpdated = latestExchange.Created
            };
        }

        private async Task<TotalDebtsDto> GetTotalDebtsData(ApplicationDbContext db)
        {
            // حساب ديون المبيعات
            var salesDebts = await db.SalesInvoices
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
                    TotalAmount = g.First().Invoice.TotalAmount - g.First().Invoice.Discount,
                    PaidAmount = g.Where(x => x.Payment != null).Sum(x => x.Payment.AmountInPaymentCurrency ?? x.Payment.Amount)
                })
                .Where(x => x.PaidAmount < x.TotalAmount)
                .SumAsync(x => x.TotalAmount - x.PaidAmount);

            // حساب ديون المشتريات
            var purchaseDebts = await db.PurchaseInvoices
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
                    TotalAmount = g.First().Invoice.TotalAmount - g.First().Invoice.Discount,
                    PaidAmount = g.Where(x => x.Payment != null).Sum(x => x.Payment.AmountInPaymentCurrency ?? x.Payment.Amount)
                })
                .Where(x => x.PaidAmount < x.TotalAmount)
                .SumAsync(x => x.TotalAmount - x.PaidAmount);

            return new TotalDebtsDto
            {
                SalesDebts = salesDebts,
                PurchaseDebts = purchaseDebts,
                TotalDebts = salesDebts + purchaseDebts
            };
        }

        private async Task<List<UnpaidInvoiceDto>> GetTopUnpaidSalesInvoicesData(ApplicationDbContext db)
        {
            return await db.SalesInvoices
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
                .Select(g => new UnpaidInvoiceDto
                {
                    InvoiceId = g.Key,
                    InvoiceNumber = g.First().Invoice.InvoiceNumber,
                    CustomerName = g.First().Invoice.Customer.Name,
                    InvoiceDate = g.First().Invoice.InvoiceDate,
                    TotalAmount = g.First().Invoice.TotalAmount - g.First().Invoice.Discount,
                    PaidAmount = g.Where(x => x.Payment != null).Sum(x => x.Payment.AmountInPaymentCurrency ?? x.Payment.Amount),
                    UnpaidAmount = g.First().Invoice.TotalAmount - g.First().Invoice.Discount - g.Where(x => x.Payment != null).Sum(x => x.Payment.Amount)
                })
                .Where(x => x.UnpaidAmount > 0)
                .OrderByDescending(x => x.UnpaidAmount)
                .Take(5)
                .ToListAsync();
        }

        private async Task<List<UnpaidInvoiceDto>> GetTopUnpaidPurchaseInvoicesData(ApplicationDbContext db)
        {
            return await db.PurchaseInvoices
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
                .Select(g => new UnpaidInvoiceDto
                {
                    InvoiceId = g.Key,
                    InvoiceNumber = g.First().Invoice.InvoiceNumber,
                    SupplierName = g.First().Invoice.Supplier.Name,
                    InvoiceDate = g.First().Invoice.InvoiceDate,
                    TotalAmount = g.First().Invoice.TotalAmount - g.First().Invoice.Discount,
                    PaidAmount = g.Where(x => x.Payment != null).Sum(x => x.Payment.AmountInPaymentCurrency ?? x.Payment.Amount),
                    UnpaidAmount = g.First().Invoice.TotalAmount - g.First().Invoice.Discount - g.Where(x => x.Payment != null).Sum(x => x.Payment.Amount)
                })
                .Where(x => x.UnpaidAmount > 0)
                .OrderByDescending(x => x.UnpaidAmount)
                .Take(5)
                .ToListAsync();
        }
    }

    // المودل الموحد للـ Dashboard
    public class DashboardDataDto
    {
        public CashBalanceDto CashBalance { get; set; }
        public DollarRateDto DollarRate { get; set; }
        public int CustomersCount { get; set; }
        public int SuppliersCount { get; set; }
        public TotalDebtsDto TotalDebts { get; set; }
        public List<UnpaidInvoiceDto> TopUnpaidSalesInvoices { get; set; }
        public List<UnpaidInvoiceDto> TopUnpaidPurchaseInvoices { get; set; }
    }

    // DTOs المساعدة
    public class CashBalanceDto
    {
        public decimal TotalSyrianBalance { get; set; }
        public decimal TotalDollarBalance { get; set; }
    }

    public class DollarRateDto
    {
        public decimal Rate { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class TotalDebtsDto
    {
        public decimal SalesDebts { get; set; }
        public decimal PurchaseDebts { get; set; }
        public decimal TotalDebts { get; set; }
    }

    public class UnpaidInvoiceDto
    {
        public long InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string SupplierName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal UnpaidAmount { get; set; }
    }
}
