using System.Threading.Tasks;
using MarketZone.Application.Wrappers;
using MarketZone.WebApi.Infrastructure.Results;
using Microsoft.AspNetCore.Http;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace MarketZone.WebApi.Infrastructure.Filters
{
    public class BaseResultEndpointFilter : IEndpointFilter
    {
        public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var result = await next(context);

            return result switch
            {
                null => HttpResults.NoContent(),
                BaseResult baseResult => baseResult.ToHttpResult(),
                _ => result
            };
        }
    }
}

