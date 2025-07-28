namespace Wisgenix.SharedKernel.Exceptions;

/// <summary>
/// Base exception for all Wisgenix application exceptions
/// </summary>
public abstract class WisgenixException : Exception
{
    /// <summary>
    /// Gets the error code for this exception
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Gets the parameters associated with this exception
    /// </summary>
    public object[] Parameters { get; }

    /// <summary>
    /// Gets the correlation ID for tracking this exception across services
    /// </summary>
    public string? CorrelationId { get; set; }

    protected WisgenixException(string errorCode, string message, params object[] parameters) 
        : base(message)
    {
        ErrorCode = errorCode;
        Parameters = parameters;
    }

    protected WisgenixException(string errorCode, string message, Exception innerException, params object[] parameters) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Parameters = parameters;
    }

    /// <summary>
    /// Gets a formatted message with parameters
    /// </summary>
    public virtual string GetFormattedMessage()
    {
        try
        {
            return Parameters.Length > 0 ? string.Format(Message, Parameters) : Message;
        }
        catch (FormatException)
        {
            return Message;
        }
    }
}
