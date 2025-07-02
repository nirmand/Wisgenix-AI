using System;

namespace Wisgenix.Core.Logger;

public static class LoggerExtensions
{
    public static void LogOperationStart<T>(this ILoggingService logger, LogContext context, string message, params object[] args)
    {
        logger.LogInformation("Starting {Operation}: {Message}",
        context.Operation, string.Format(message, args));
    }

    public static void LogOperationSuccess<T>(this ILoggingService logger, LogContext context, string message, params object[] args)
    {
        logger.LogInformation("Completed {Operation}: {Message}",
        context.Operation, string.Format(message, args));
    }

    public static void LogOperationError<T>(this ILoggingService logger, LogContext context, Exception ex, string message, params object[] args)
    {
        logger.LogError(ex, "Error in {Operation}: {Message}",
        context.Operation, string.Format(message, args));
    }

    public static void LogOperationWarning<T>(this ILoggingService logger, LogContext context, string message, params object[] args)
    {
        logger.LogWarning("Warning in {Operation}: {Message}",
        context.Operation, string.Format(message, args));
    }
}