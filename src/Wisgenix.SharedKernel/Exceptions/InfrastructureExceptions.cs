namespace Wisgenix.SharedKernel.Exceptions;

/// <summary>
/// Base exception for infrastructure-layer errors
/// </summary>
public abstract class InfrastructureException : WisgenixException
{
    protected InfrastructureException(string errorCode, string message, params object[] parameters) 
        : base(errorCode, message, parameters)
    {
    }

    protected InfrastructureException(string errorCode, string message, Exception innerException, params object[] parameters) 
        : base(errorCode, message, innerException, parameters)
    {
    }
}

/// <summary>
/// Exception thrown when external service calls fail
/// </summary>
public class ExternalServiceException : InfrastructureException
{
    /// <summary>
    /// Gets the name of the external service that failed
    /// </summary>
    public string ServiceName { get; }

    /// <summary>
    /// Gets the HTTP status code if applicable
    /// </summary>
    public int? StatusCode { get; }

    public ExternalServiceException(string serviceName, string message, params object[] parameters) 
        : base("EXTERNAL_SERVICE_ERROR", message, parameters)
    {
        ServiceName = serviceName;
    }

    public ExternalServiceException(string serviceName, int statusCode, string message, params object[] parameters) 
        : base("EXTERNAL_SERVICE_ERROR", message, parameters)
    {
        ServiceName = serviceName;
        StatusCode = statusCode;
    }

    public ExternalServiceException(string serviceName, string message, Exception innerException, params object[] parameters) 
        : base("EXTERNAL_SERVICE_ERROR", message, innerException, parameters)
    {
        ServiceName = serviceName;
    }
}

/// <summary>
/// Exception thrown when database operations fail
/// </summary>
public class DatabaseException : InfrastructureException
{
    /// <summary>
    /// Gets the operation that failed
    /// </summary>
    public string Operation { get; }

    public DatabaseException(string operation, string message, params object[] parameters) 
        : base("DATABASE_ERROR", message, parameters)
    {
        Operation = operation;
    }

    public DatabaseException(string operation, string message, Exception innerException, params object[] parameters) 
        : base("DATABASE_ERROR", message, innerException, parameters)
    {
        Operation = operation;
    }
}

/// <summary>
/// Exception thrown when configuration is invalid or missing
/// </summary>
public class ConfigurationException : InfrastructureException
{
    /// <summary>
    /// Gets the configuration key that is invalid or missing
    /// </summary>
    public string ConfigurationKey { get; }

    public ConfigurationException(string configurationKey, string message, params object[] parameters) 
        : base("CONFIGURATION_ERROR", message, parameters)
    {
        ConfigurationKey = configurationKey;
    }

    public ConfigurationException(string configurationKey, string message, Exception innerException, params object[] parameters) 
        : base("CONFIGURATION_ERROR", message, innerException, parameters)
    {
        ConfigurationKey = configurationKey;
    }
}
