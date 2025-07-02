using Microsoft.AspNetCore.Mvc;
using Wisgenix.Core.Logger;

namespace Wisgenix.API.Controllers
{
    public abstract class BaseApiController : ControllerBase
    {
        protected LogContext CreateLogContext(string operation, string? userName = null)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
            return LogContext.Create(operation, userName ?? User?.Identity?.Name ?? "system", correlationId);
        }
    }
}
