namespace Wisgenix.SharedKernel.Infrastructure.Logging;

/// <summary>
/// Logging service interface for structured logging
/// </summary>
public interface ILoggingService
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
    void LogDebug(string message, params object[] args);
    void LogTrace(string message, params object[] args);
    
    void LogOperationStart<T>(LogContext context, string message, params object[] args);
    void LogOperationSuccess<T>(LogContext context, string message, params object[] args);
    void LogOperationWarning<T>(LogContext context, string message, params object[] args);
    void LogOperationError<T>(LogContext context, Exception exception, string message, params object[] args);
}

/// <summary>
/// Context for logging operations with correlation tracking
/// </summary>
public class LogContext
{
    public string Operation { get; }
    public string? UserName { get; }
    public string? CorrelationId { get; }
    public DateTime Timestamp { get; }

    private LogContext(string operation, string? userName, string? correlationId)
    {
        Operation = operation;
        UserName = userName;
        CorrelationId = correlationId;
        Timestamp = DateTime.UtcNow;
    }

    public static LogContext Create(string operation, string? userName = null, string? correlationId = null)
    {
        return new LogContext(operation, userName, correlationId ?? Guid.NewGuid().ToString());
    }
}
