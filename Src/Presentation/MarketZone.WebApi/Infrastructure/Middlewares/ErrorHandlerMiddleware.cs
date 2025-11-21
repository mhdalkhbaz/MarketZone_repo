using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using Microsoft.AspNetCore.Http;

namespace MarketZone.WebApi.Infrastructure.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITranslator _translator;

        public ErrorHandlerMiddleware(RequestDelegate next, ITranslator translator)
        {
            _next = next;
            _translator = translator;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = BaseResult.Failure();

                switch (error)
                {
                    case ValidationException e:
                        // validation error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        foreach (var validationFailure in e.Errors)
                        {
                            responseModel.AddError(new Error(ErrorCode.ModelStateNotValid, validationFailure.ErrorMessage, validationFailure.PropertyName));
                        }
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        var notFoundMessage = _translator.GetString("Resource_NotFound") ?? e.Message;
                        responseModel.AddError(new Error(ErrorCode.NotFound, notFoundMessage));
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var errorMessage = _translator.GetString("Internal_Server_Error") ?? error.Message;
                        responseModel.AddError(new Error(ErrorCode.Exception, errorMessage));
                        break;
                }
                var result = JsonSerializer.Serialize(responseModel, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await response.WriteAsync(result);
            }
        }
    }
}
