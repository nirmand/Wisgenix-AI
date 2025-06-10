using System;

namespace Wisgenix.Core.Logger;

public class LogContext
{
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    public string Operation { get; set; } = string.Empty;
    public string? UserName { get; set; }

    public static LogContext Create(string operation, string? userName = null)
    {
        return new LogContext
        {
            CorrelationId = Guid.NewGuid().ToString(),
            Operation = operation,
            UserName = userName
        };
    }
}