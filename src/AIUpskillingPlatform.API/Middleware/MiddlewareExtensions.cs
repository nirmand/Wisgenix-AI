using System;

namespace AIUpskillingPlatform.API.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseValidationLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ValidationLoggingMiddleware>();
    }
}
