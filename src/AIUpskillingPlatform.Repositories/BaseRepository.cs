using AIUpskillingPlatform.Core.Logger;
using AIUpskillingPlatform.Data;

namespace AIUpskillingPlatform.Repositories;

public abstract class BaseRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext Context;
    protected readonly ILoggingService Logger;

    protected BaseRepository(AppDbContext context, ILoggingService logger)
    {
        Context = context;
        Logger = logger;
    }

    protected async Task<TResult> ExecuteWithLoggingAsync<TResult>(
        LogContext logContext,
        string operationName,
        Func<Task<TResult>> operation,
        Func<TResult, string>? successMessageFactory = null)
    {
        try
        {
            Logger.LogOperationStart<TEntity>(logContext, $"{operationName} started");
            var result = await operation();
            Logger.LogOperationSuccess<TEntity>(
                logContext, 
                successMessageFactory?.Invoke(result) ?? $"{operationName} completed successfully");
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogOperationError<TEntity>(logContext, ex, $"Error in {operationName}: {ex.Message}");
            throw;
        }
    }

    protected async Task ExecuteWithLoggingAsync(
        LogContext logContext,
        string operationName,
        Func<Task> operation)
    {
        try
        {
            Logger.LogOperationStart<TEntity>(logContext, $"{operationName} started");
            await operation();
            Logger.LogOperationSuccess<TEntity>(logContext, $"{operationName} completed successfully");
        }
        catch (Exception ex)
        {
            Logger.LogOperationError<TEntity>(logContext, ex, $"Error in {operationName}: {ex.Message}");
            throw;
        }
    }
}