namespace Wisgenix.SharedKernel.Exceptions;

/// <summary>
/// Base exception for application-layer errors
/// </summary>
public abstract class ApplicationException : WisgenixException
{
    protected ApplicationException(string errorCode, string message, params object[] parameters) 
        : base(errorCode, message, parameters)
    {
    }

    protected ApplicationException(string errorCode, string message, Exception innerException, params object[] parameters) 
        : base(errorCode, message, innerException, parameters)
    {
    }
}

/// <summary>
/// Exception thrown when validation fails at the application layer
/// </summary>
public class ValidationException : ApplicationException
{
    /// <summary>
    /// Gets the validation errors
    /// </summary>
    public Dictionary<string, string[]> ValidationErrors { get; }

    public ValidationException(Dictionary<string, string[]> validationErrors) 
        : base("VALIDATION_ERROR", "One or more validation errors occurred")
    {
        ValidationErrors = validationErrors;
    }

    public ValidationException(string propertyName, string errorMessage) 
        : base("VALIDATION_ERROR", "Validation failed for {0}: {1}", propertyName, errorMessage)
    {
        ValidationErrors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        };
    }

    public ValidationException(string message, params object[] parameters) 
        : base("VALIDATION_ERROR", message, parameters)
    {
        ValidationErrors = new Dictionary<string, string[]>();
    }
}

/// <summary>
/// Exception thrown when user is not authorized
/// </summary>
public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(string message = "User is not authorized to perform this action", params object[] parameters) 
        : base("UNAUTHORIZED", message, parameters)
    {
    }

    public UnauthorizedException(string message, Exception innerException, params object[] parameters) 
        : base("UNAUTHORIZED", message, innerException, parameters)
    {
    }
}

/// <summary>
/// Exception thrown when user is forbidden from accessing a resource
/// </summary>
public class ForbiddenException : ApplicationException
{
    public ForbiddenException(string message = "Access to this resource is forbidden", params object[] parameters) 
        : base("FORBIDDEN", message, parameters)
    {
    }

    public ForbiddenException(string message, Exception innerException, params object[] parameters) 
        : base("FORBIDDEN", message, innerException, parameters)
    {
    }
}

/// <summary>
/// Exception thrown when a requested operation conflicts with the current state
/// </summary>
public class ConflictException : ApplicationException
{
    public ConflictException(string message, params object[] parameters) 
        : base("CONFLICT", message, parameters)
    {
    }

    public ConflictException(string message, Exception innerException, params object[] parameters) 
        : base("CONFLICT", message, innerException, parameters)
    {
    }
}
