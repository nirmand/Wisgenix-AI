using System.Text.Json;
using Wisgenix.Core.Logger;
using Microsoft.AspNetCore.Mvc;

namespace Wisgenix.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Try to get the endpoint and model state
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var actionDescriptor = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
                if (actionDescriptor != null)
                {
                    _logger.LogInformation("RequestLoggingMiddleware triggered for {Method} {Path}", context.Request.Method, context.Request.Path);
                }
            }
            
            await _next(context);
        }
    }
}