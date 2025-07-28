namespace Wisgenix.SharedKernel.Exceptions;

/// <summary>
/// Base exception for domain-related errors
/// </summary>
public abstract class DomainException : WisgenixException
{
    protected DomainException(string errorCode, string message, params object[] parameters) 
        : base(errorCode, message, parameters)
    {
    }

    protected DomainException(string errorCode, string message, Exception innerException, params object[] parameters) 
        : base(errorCode, message, innerException, parameters)
    {
    }
}

/// <summary>
/// Exception thrown when domain validation fails
/// </summary>
public class DomainValidationException : DomainException
{
    public DomainValidationException(string message, params object[] parameters) 
        : base("DOMAIN_VALIDATION_ERROR", message, parameters)
    {
    }

    public DomainValidationException(string message, Exception innerException, params object[] parameters) 
        : base("DOMAIN_VALIDATION_ERROR", message, innerException, parameters)
    {
    }
}

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message, params object[] parameters) 
        : base("BUSINESS_RULE_VIOLATION", message, parameters)
    {
    }

    public BusinessRuleViolationException(string message, Exception innerException, params object[] parameters) 
        : base("BUSINESS_RULE_VIOLATION", message, innerException, parameters)
    {
    }
}

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id) 
        : base("ENTITY_NOT_FOUND", "{0} with id '{1}' was not found", entityName, id)
    {
    }

    public EntityNotFoundException(string message, params object[] parameters) 
        : base("ENTITY_NOT_FOUND", message, parameters)
    {
    }
}

/// <summary>
/// Exception thrown when a duplicate entity is detected
/// </summary>
public class DuplicateEntityException : DomainException
{
    public DuplicateEntityException(string entityName, string duplicateField, object value) 
        : base("DUPLICATE_ENTITY", "{0} with {1} '{2}' already exists", entityName, duplicateField, value)
    {
    }

    public DuplicateEntityException(string message, params object[] parameters) 
        : base("DUPLICATE_ENTITY", message, parameters)
    {
    }
}
