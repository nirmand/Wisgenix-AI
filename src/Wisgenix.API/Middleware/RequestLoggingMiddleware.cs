using System.Text.Json;
using Wisgenix.Core.Logger;
using Microsoft.AspNetCore.Mvc;
using SerilogContext = Serilog.Context.LogContext;

namespace Wisgenix.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private const string CorrelationIdHeader = "X-Correlation-ID";
        private const string CorrelationIdItemKey = "CorrelationId";

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Get or create correlation ID
            var correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var cid)
                ? cid.ToString()
                : Guid.NewGuid().ToString();

            // Store in HttpContext.Items for downstream access
            context.Items[CorrelationIdItemKey] = correlationId;
            // Add to response header
            context.Response.Headers[CorrelationIdHeader] = correlationId;

            using (SerilogContext.PushProperty("CorrelationId", correlationId))
            {
                // Try to get the endpoint and model state
                var endpoint = context.GetEndpoint();
                if (endpoint != null)
                {
                    var actionDescriptor = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
                    if (actionDescriptor != null)
                    {
                        _logger.LogInformation("[{CorrelationId}] RequestLoggingMiddleware triggered for {Method} {Path}",
                            correlationId, context.Request.Method, context.Request.Path);
                    }
                }
                
                await _next(context);
            }
        }
    }
}