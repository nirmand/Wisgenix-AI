using Content.API.Models;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Wisgenix.SharedKernel.Exceptions;

namespace Content.API.Middleware;

/// <summary>
/// Global exception handling middleware that catches all unhandled exceptions
/// and returns standardized error responses
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        // Log the exception with correlation ID
        LogException(exception, correlationId, traceId, context);

        // Create error response
        var errorResponse = CreateErrorResponse(exception, correlationId, traceId);
        var statusCode = GetStatusCode(exception);

        // Set response properties
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        // Serialize and write response
        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private ErrorResponse CreateErrorResponse(Exception exception, string correlationId, string traceId)
    {
        return exception switch
        {
            WisgenixException wisgenixException => ErrorResponse.FromException(wisgenixException, correlationId, traceId),
            
            // Handle FluentValidation exceptions
            FluentValidation.ValidationException fluentValidationException => ErrorResponse.CreateValidationError(
                fluentValidationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()),
                correlationId),
            
            // Handle other common exceptions (order matters - more specific first)
            ArgumentNullException argumentNullException => ErrorResponse.Create(
                "INVALID_ARGUMENT",
                $"Required parameter '{argumentNullException.ParamName}' is null",
                correlationId),

            ArgumentException argumentException => ErrorResponse.Create(
                "INVALID_ARGUMENT",
                argumentException.Message,
                correlationId),
            
            InvalidOperationException invalidOperationException => ErrorResponse.Create(
                "INVALID_OPERATION", 
                invalidOperationException.Message, 
                correlationId),
            
            // Default case for unhandled exceptions
            _ => ErrorResponse.Create(
                "INTERNAL_SERVER_ERROR", 
                _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred", 
                correlationId)
        };
    }

    private static HttpStatusCode GetStatusCode(Exception exception)
    {
        return exception switch
        {
            DomainValidationException => HttpStatusCode.BadRequest,
            BusinessRuleViolationException => HttpStatusCode.BadRequest,
            EntityNotFoundException => HttpStatusCode.NotFound,
            DuplicateEntityException => HttpStatusCode.Conflict,
            ValidationException => HttpStatusCode.BadRequest,
            UnauthorizedException => HttpStatusCode.Unauthorized,
            ForbiddenException => HttpStatusCode.Forbidden,
            ConflictException => HttpStatusCode.Conflict,
            ExternalServiceException => HttpStatusCode.BadGateway,
            DatabaseException => HttpStatusCode.InternalServerError,
            ConfigurationException => HttpStatusCode.InternalServerError,
            FluentValidation.ValidationException => HttpStatusCode.BadRequest,
            ArgumentNullException => HttpStatusCode.BadRequest,
            ArgumentException => HttpStatusCode.BadRequest,
            InvalidOperationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };
    }

    private void LogException(Exception exception, string correlationId, string traceId, HttpContext context)
    {
        var logLevel = GetLogLevel(exception);
        var message = "Unhandled exception occurred. CorrelationId: {CorrelationId}, TraceId: {TraceId}, Path: {Path}";
        
        _logger.Log(logLevel, exception, message, correlationId, traceId, context.Request.Path);
    }

    private static LogLevel GetLogLevel(Exception exception)
    {
        return exception switch
        {
            DomainValidationException => LogLevel.Warning,
            BusinessRuleViolationException => LogLevel.Warning,
            EntityNotFoundException => LogLevel.Warning,
            ValidationException => LogLevel.Warning,
            UnauthorizedException => LogLevel.Warning,
            ForbiddenException => LogLevel.Warning,
            ArgumentNullException => LogLevel.Warning,
            ArgumentException => LogLevel.Warning,
            FluentValidation.ValidationException => LogLevel.Warning,
            _ => LogLevel.Error
        };
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        const string correlationIdHeader = "X-Correlation-ID";
        
        if (context.Request.Headers.TryGetValue(correlationIdHeader, out var correlationId) && 
            !string.IsNullOrEmpty(correlationId))
        {
            return correlationId.ToString();
        }

        var newCorrelationId = Guid.NewGuid().ToString();
        context.Response.Headers.TryAdd(correlationIdHeader, newCorrelationId);
        return newCorrelationId;
    }
}
