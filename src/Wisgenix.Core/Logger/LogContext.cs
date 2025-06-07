using System;

namespace Wisgenix.Core.Logger;

public class LogContext
{
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    public string Operation { get; set; } = string.Empty;

    public static LogContext Create(string operation)
    {
        return new LogContext
        {
            CorrelationId = Guid.NewGuid().ToString(),
            Operation = operation
        };
    }
}