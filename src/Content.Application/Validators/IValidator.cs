namespace Content.Application.Validators;

/// <summary>
/// Generic validator interface
/// </summary>
/// <typeparam name="T">Type to validate</typeparam>
public interface IValidator<in T>
{
    Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}

/// <summary>
/// Validation result containing validation outcome and errors
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; }
    public IEnumerable<ValidationError> Errors { get; }

    public ValidationResult(bool isValid, IEnumerable<ValidationError> errors)
    {
        IsValid = isValid;
        Errors = errors ?? Enumerable.Empty<ValidationError>();
    }

    public static ValidationResult Success() => new(true, Enumerable.Empty<ValidationError>());
    public static ValidationResult Failure(IEnumerable<ValidationError> errors) => new(false, errors);
    public static ValidationResult Failure(params ValidationError[] errors) => new(false, errors);
}

/// <summary>
/// Represents a validation error
/// </summary>
public class ValidationError
{
    public string PropertyName { get; }
    public string ErrorMessage { get; }

    public ValidationError(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }
}
