using AIUpskillingPlatform.Core.Logger;
using FluentValidation.Results;

namespace AIUpskillingPlatform.API.Middleware
{
    public class ValidationLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggingService _logger;

        public ValidationLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILoggingService logger)
        {
            var logContext = LogContext.Create("ValidationMiddleware");

            if (!context.Items.ContainsKey("ValidationErrors") || 
                context.Items["ValidationErrors"] is not IEnumerable<ValidationFailure> errors) 
            {
                await _next(context);
                return;
            }

            foreach (var error in errors)
            {
                _logger.LogOperationWarning(
                    logContext,
                    "Validation failed for property {Property}: {Error}",
                    error.PropertyName,
                    error.ErrorMessage
                );
            }

            await _next(context);
        }
    }

}
