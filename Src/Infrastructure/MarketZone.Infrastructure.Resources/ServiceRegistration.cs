using MarketZone.Application.Interfaces;
using MarketZone.Infrastructure.Resources.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MarketZone.Infrastructure.Resources
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddResourcesInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<ITranslator, Translator>();

            return services;
        }
    }
}
