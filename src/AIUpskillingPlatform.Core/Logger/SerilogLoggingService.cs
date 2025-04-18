using System;
using Serilog;

namespace AIUpskillingPlatform.Core.Logger;

public class SerilogLoggingService : ILoggingService
{
    private readonly ILogger _logger;

    public SerilogLoggingService(ILogger logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message, params object[] args)
        => _logger.Information(message, args);

    public void LogWarning(string message, params object[] args)
        => _logger.Warning(message, args);

    public void LogError(Exception exception, string message, params object[] args)
        => _logger.Error(exception, message, args);

    public void LogDebug(string message, params object[] args)
        => _logger.Debug(message, args);
}
