using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace MarketZone.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
