using System.Text.Json.Serialization;

namespace Content.API.Models;

/// <summary>
/// Standardized error response model for API endpoints
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets or sets the error code
    /// </summary>
    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the correlation ID for tracking
    /// </summary>
    [JsonPropertyName("correlationId")]
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the error occurred
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets validation errors (for validation failures)
    /// </summary>
    [JsonPropertyName("validationErrors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    /// <summary>
    /// Gets or sets the trace ID for debugging
    /// </summary>
    [JsonPropertyName("traceId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets additional details about the error
    /// </summary>
    [JsonPropertyName("details")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Details { get; set; }

    /// <summary>
    /// Creates a basic error response
    /// </summary>
    public static ErrorResponse Create(string errorCode, string message, string correlationId)
    {
        return new ErrorResponse
        {
            ErrorCode = errorCode,
            Message = message,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an error response with validation errors
    /// </summary>
    public static ErrorResponse CreateValidationError(Dictionary<string, string[]> validationErrors, string correlationId)
    {
        return new ErrorResponse
        {
            ErrorCode = "VALIDATION_ERROR",
            Message = "One or more validation errors occurred",
            CorrelationId = correlationId,
            ValidationErrors = validationErrors,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an error response from a WisgenixException
    /// </summary>
    public static ErrorResponse FromException(Wisgenix.SharedKernel.Exceptions.WisgenixException exception, string correlationId, string? traceId = null)
    {
        var response = new ErrorResponse
        {
            ErrorCode = exception.ErrorCode,
            Message = exception.GetFormattedMessage(),
            CorrelationId = correlationId,
            TraceId = traceId,
            Timestamp = DateTime.UtcNow
        };

        // Add validation errors if it's a ValidationException
        if (exception is Wisgenix.SharedKernel.Exceptions.ValidationException validationException)
        {
            response.ValidationErrors = validationException.ValidationErrors;
        }

        return response;
    }
}
