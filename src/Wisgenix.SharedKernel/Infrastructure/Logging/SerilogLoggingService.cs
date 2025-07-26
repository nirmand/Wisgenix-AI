using Serilog;

namespace Wisgenix.SharedKernel.Infrastructure.Logging;

/// <summary>
/// Serilog implementation of the logging service
/// </summary>
public class SerilogLoggingService : ILoggingService
{
    private readonly ILogger _logger;

    public SerilogLoggingService(ILogger logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.Information(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.Warning(message, args);
    }

    public void LogError(string message, params object[] args)
    {
        _logger.Error(message, args);
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        _logger.Error(exception, message, args);
    }

    public void LogDebug(string message, params object[] args)
    {
        _logger.Debug(message, args);
    }

    public void LogTrace(string message, params object[] args)
    {
        _logger.Verbose(message, args);
    }

    public void LogOperationStart<T>(LogContext context, string message, params object[] args)
    {
        _logger.ForContext("Operation", context.Operation)
               .ForContext("UserName", context.UserName)
               .ForContext("CorrelationId", context.CorrelationId)
               .ForContext("EntityType", typeof(T).Name)
               .Information("Starting {Operation}: " + message, args);
    }

    public void LogOperationSuccess<T>(LogContext context, string message, params object[] args)
    {
        _logger.ForContext("Operation", context.Operation)
               .ForContext("UserName", context.UserName)
               .ForContext("CorrelationId", context.CorrelationId)
               .ForContext("EntityType", typeof(T).Name)
               .Information("Success {Operation}: " + message, args);
    }

    public void LogOperationWarning<T>(LogContext context, string message, params object[] args)
    {
        _logger.ForContext("Operation", context.Operation)
               .ForContext("UserName", context.UserName)
               .ForContext("CorrelationId", context.CorrelationId)
               .ForContext("EntityType", typeof(T).Name)
               .Warning("Warning {Operation}: " + message, args);
    }

    public void LogOperationError<T>(LogContext context, Exception exception, string message, params object[] args)
    {
        _logger.ForContext("Operation", context.Operation)
               .ForContext("UserName", context.UserName)
               .ForContext("CorrelationId", context.CorrelationId)
               .ForContext("EntityType", typeof(T).Name)
               .Error(exception, "Error {Operation}: " + message, args);
    }
}
