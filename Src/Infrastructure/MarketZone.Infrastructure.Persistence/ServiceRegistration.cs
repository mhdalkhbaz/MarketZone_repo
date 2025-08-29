using System.Linq;
using System.Reflection;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Infrastructure.Persistence.Contexts;
using MarketZone.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MarketZone.Application.Interfaces.Services;
using MarketZone.Infrastructure.Persistence.Services;

namespace MarketZone.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration, bool useInMemoryDatabase)
        {
            if (useInMemoryDatabase)
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase(nameof(ApplicationDbContext)));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            }

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.RegisterRepositories();
            services.AddTransient<IPurchaseInvoiceNumberGenerator, PurchaseInvoiceNumberGenerator>();
            services.AddTransient<ISalesInvoiceNumberGenerator, SalesInvoiceNumberGenerator>();
            services.AddTransient<IInventoryAdjustmentService, InventoryAdjustmentService>();
            services.AddTransient<ISalesInventoryService, SalesInventoryService>();
            services.AddTransient<IRoastingService, RoastingService>();
            services.AddTransient<IRoastingInvoiceNumberGenerator, RoastingInvoiceNumberGenerator>();

            return services;
        }
        private static void RegisterRepositories(this IServiceCollection services)
        {
            // Register all other repositories using reflection
            var repositoryInterfaces = Assembly.GetAssembly(typeof(IGenericRepository<>))!.GetTypes()
                .Where(t => t.IsInterface && t.GetInterface(typeof(IGenericRepository<>).Name) != null);

            var repositoryImplementations = Assembly.GetAssembly(typeof(GenericRepository<>))!.GetTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract);

            foreach (var repositoryInterface in repositoryInterfaces)
            {
                var implementation = repositoryImplementations.FirstOrDefault(t => 
                    repositoryInterface.IsAssignableFrom(t) && t != typeof(GenericRepository<>));

                if (implementation != null)
                {
                    services.AddTransient(repositoryInterface, implementation);
                }
            }
        }
    }
}
