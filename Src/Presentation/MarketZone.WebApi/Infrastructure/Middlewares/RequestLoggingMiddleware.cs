using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Serilog;

namespace MarketZone.WebApi.Infrastructure.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private const int MaxBodyLength = 10000; // Limit body logging to 10KB to avoid performance issues

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip logging for health checks and swagger
            if (context.Request.Path.StartsWithSegments("/health") ||
                context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            // Enable buffering to allow reading the request body multiple times
            context.Request.EnableBuffering();

            // Capture request details
            var requestBody = await ReadRequestBodyAsync(context.Request);
            var requestPath = context.Request.Path;
            var requestMethod = context.Request.Method;
            var requestQueryString = context.Request.QueryString.ToString();
            var requestHeaders = FormatHeaders(context.Request.Headers);
            var requestUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{requestQueryString}";

            // Truncate body if too long
            if (requestBody.Length > MaxBodyLength)
            {
                requestBody = requestBody.Substring(0, MaxBodyLength) + "... [TRUNCATED]";
            }

            // Log the incoming request
            Log.Information(
                "=== Incoming Request ===\n" +
                "Method: {Method}\n" +
                "URL: {Url}\n" +
                "Path: {Path}\n" +
                "QueryString: {QueryString}\n" +
                "Headers:\n{Headers}\n" +
                "Body:\n{RequestBody}\n" +
                "========================",
                requestMethod,
                requestUrl,
                requestPath,
                requestQueryString,
                requestHeaders,
                string.IsNullOrWhiteSpace(requestBody) ? "[Empty]" : requestBody);

            // Capture response
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                // Capture response details
                var responseBodyContent = await ReadResponseBodyAsync(context.Response);
                var statusCode = context.Response.StatusCode;

                // Truncate response body if too long
                if (responseBodyContent.Length > MaxBodyLength)
                {
                    responseBodyContent = responseBodyContent.Substring(0, MaxBodyLength) + "... [TRUNCATED]";
                }

                // Log the response
                Log.Information(
                    "=== Outgoing Response ===\n" +
                    "Method: {Method}\n" +
                    "URL: {Url}\n" +
                    "Status Code: {StatusCode}\n" +
                    "Response Body:\n{ResponseBody}\n" +
                    "=========================",
                    requestMethod,
                    requestUrl,
                    statusCode,
                    string.IsNullOrWhiteSpace(responseBodyContent) ? "[Empty]" : responseBodyContent);

                // Copy the response back to the original stream
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            try
            {
                if (request.Body.CanSeek)
                {
                    request.Body.Position = 0;
                }

                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();

                if (request.Body.CanSeek)
                {
                    request.Body.Position = 0;
                }

                return body;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to read request body");
                return "[Unable to read request body]";
            }
        }

        private async Task<string> ReadResponseBodyAsync(HttpResponse response)
        {
            try
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                var body = await new StreamReader(response.Body, Encoding.UTF8).ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);
                return body;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to read response body");
                return "[Unable to read response body]";
            }
        }

        private string FormatHeaders(IHeaderDictionary headers)
        {
            var sb = new StringBuilder();
            foreach (var header in headers)
            {
                // Skip sensitive headers
                if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) ||
                    header.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine($"  {header.Key}: [REDACTED]");
                }
                else
                {
                    //sb.AppendLine($"  {header.Key}: {String.Join(", ", header.Value)}");
                }
            }
            return sb.ToString();
        }
    }
}

