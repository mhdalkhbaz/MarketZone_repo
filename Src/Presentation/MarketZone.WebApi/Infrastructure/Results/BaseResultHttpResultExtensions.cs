using System.Linq;
using MarketZone.Application.Wrappers;
using Microsoft.AspNetCore.Http;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace MarketZone.WebApi.Infrastructure.Results
{
    public static class BaseResultHttpResultExtensions
    {
        public static IResult ToHttpResult(this BaseResult baseResult)
        {
            if (baseResult is null)
                return HttpResults.NoContent();

            if (baseResult.Success)
                return HttpResults.Ok(baseResult);

            var status = MapStatusCode(baseResult);
            return HttpResults.Json(baseResult, statusCode: status);
        }

        private static int MapStatusCode(BaseResult baseResult)
        {
            var errorCode = baseResult.Errors?.FirstOrDefault()?.ErrorCode;

            return errorCode switch
            {
                ErrorCode.NotFound => StatusCodes.Status404NotFound,
                ErrorCode.AccessDenied => StatusCodes.Status403Forbidden,
                ErrorCode.Exception => StatusCodes.Status500InternalServerError,
                ErrorCode.FieldDataInvalid => StatusCodes.Status400BadRequest,
                ErrorCode.ModelStateNotValid => StatusCodes.Status400BadRequest,
                ErrorCode.ErrorInIdentity => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status400BadRequest
            };
        }
    }
}

