using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Domain.Categories.Entities;
using MarketZone.Domain.Customers.Entities;
using MarketZone.Domain.Employees.Entities;
using MarketZone.Domain.Products.Entities;
using MarketZone.Domain.Suppliers.Entities;
using MarketZone.Domain.Purchases.Entities;
using MarketZone.Domain.Sales.Entities;
using MarketZone.Domain.Inventory.Entities;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Roasting.Entities;
using MarketZone.Domain.Logistics.Entities;
using MarketZone.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Contexts
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IAuthenticatedUserService authenticatedUser) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeSalary> EmployeeSalaries { get; set; }
        public DbSet<SalaryPayment> SalaryPayments { get; set; }
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
        public DbSet<PurchaseInvoiceDetail> PurchaseInvoiceDetails { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesInvoiceDetail> SalesInvoiceDetails { get; set; }
        public DbSet<InventoryHistory> InventoryHistories { get; set; }
        public DbSet<ProductBalance> ProductBalances { get; set; }
        public DbSet<CashRegister> CashRegisters { get; set; }
        public DbSet<CashTransaction> CashTransactions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<ExchangeTransaction> ExchangeTransactions { get; set; }
        public DbSet<RoastingInvoice> RoastingInvoices { get; set; }
        public DbSet<RoastingInvoiceDetail> RoastingInvoiceDetails { get; set; }
        public DbSet<RoastingInvoiceDetailReceipt> RoastingInvoiceDetailReceipts { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<DistributionTrip> DistributionTrips { get; set; }
        public DbSet<DistributionTripDetail> DistributionTripDetails { get; set; }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            ChangeTracker.ApplyAuditing(authenticatedUser);

            return base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            this.ConfigureDecimalProperties(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(builder);
        }
    }
}