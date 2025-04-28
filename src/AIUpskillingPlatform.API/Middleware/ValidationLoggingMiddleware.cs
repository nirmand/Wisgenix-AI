using System.Text.Json;
using AIUpskillingPlatform.Core.Logger;
using Microsoft.AspNetCore.Mvc;

namespace AIUpskillingPlatform.API.Middleware
{
    public class ValidationLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILoggingService logger)
        {
            var logContext = LogContext.Create("ValidationMiddleware");
            var originalBodyStream = context.Response.Body;

            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            try
            {
                await _next(context);

                if (context.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    using var streamReader = new StreamReader(memoryStream, leaveOpen: true);
                    var responseBody = await streamReader.ReadToEndAsync();

                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        try
                        {
                            var validationProblem = JsonSerializer.Deserialize<ValidationProblemDetails>(responseBody);
                            if (validationProblem?.Errors != null)
                            {
                                foreach (var error in validationProblem.Errors)
                                {
                                    logger.LogOperationWarning<ValidationLog>(
                                        logContext,
                                        $"Validation failed for {error.Key}: {string.Join(", ", error.Value)}"
                                    );
                                }
                            }
                        }
                        catch (JsonException ex)
                        {
                            logger.LogOperationError<ValidationLog>(
                                logContext,
                                ex,
                                "Failed to parse validation response"
                            );
                        }
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBodyStream);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }

    public static class ValidationLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseValidationLogging(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidationLoggingMiddleware>();
        }
    }
}