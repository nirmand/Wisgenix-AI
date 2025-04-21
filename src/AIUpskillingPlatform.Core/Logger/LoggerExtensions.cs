using System;

namespace AIUpskillingPlatform.Core.Logger;

public static class LoggerExtensions
{
    public static void LogOperationStart<T>(this ILoggingService logger, LogContext context, string message, params object[] args)
    {
        logger.LogInformation("[{CorrelationId}] Starting {Operation}: {Message}", 
            context.CorrelationId, context.Operation, string.Format(message, args));
    }

    public static void LogOperationSuccess<T>(this ILoggingService logger, LogContext context, string message, params object[] args)
    {
        logger.LogInformation("[{CorrelationId}] Completed {Operation}: {Message}", 
            context.CorrelationId, context.Operation, string.Format(message, args));
    }

    public static void LogOperationError<T>(this ILoggingService logger, LogContext context, Exception ex, string message, params object[] args)
    {
        logger.LogError(ex, "[{CorrelationId}] Error in {Operation}: {Message}", 
            context.CorrelationId, context.Operation, string.Format(message, args));
    }

    public static void LogOperationWarning(this ILoggingService logger, LogContext context, string message, params object[] args)
    {
        logger.LogWarning("[{CorrelationId}] Warning in {Operation}: {Message}",
        context.CorrelationId, context.Operation, string.Format(message, args));
    }
}