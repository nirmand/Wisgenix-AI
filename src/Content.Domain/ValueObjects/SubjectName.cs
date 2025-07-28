using Wisgenix.SharedKernel.Domain.Exceptions;

namespace Content.Domain.ValueObjects;

/// <summary>
/// Value object representing a subject name with validation rules
/// </summary>
public class SubjectName : ValueObject
{
    public const int MaxLength = 200;
    private static readonly char[] InvalidCharacters = { '>', '<', '&', '"', '\'' };

    public string Value { get; private set; }

    private SubjectName(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new SubjectName with validation
    /// </summary>
    /// <param name="value">The subject name value</param>
    /// <returns>A valid SubjectName instance</returns>
    /// <exception cref="BusinessRuleViolationException">Thrown when validation fails</exception>
    public static SubjectName Create(string value)
    {
        ValidateSubjectName(value);
        return new SubjectName(value.Trim());
    }

    /// <summary>
    /// Validates a subject name value
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <exception cref="BusinessRuleViolationException">Thrown when validation fails</exception>
    private static void ValidateSubjectName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleViolationException("Subject name cannot be empty");
        }

        if (value.Length > MaxLength)
        {
            throw new BusinessRuleViolationException($"Subject name cannot exceed {MaxLength} characters");
        }

        if (ContainsInvalidCharacters(value))
        {
            throw new BusinessRuleViolationException("Subject name contains invalid characters");
        }
    }

    /// <summary>
    /// Checks if the value contains invalid characters
    /// </summary>
    private static bool ContainsInvalidCharacters(string input)
    {
        return input.Any(c => InvalidCharacters.Contains(c));
    }

    /// <summary>
    /// Tries to create a SubjectName without throwing exceptions
    /// </summary>
    /// <param name="value">The subject name value</param>
    /// <param name="subjectName">The created SubjectName if successful</param>
    /// <returns>True if creation was successful, false otherwise</returns>
    public static bool TryCreate(string value, out SubjectName? subjectName)
    {
        try
        {
            subjectName = Create(value);
            return true;
        }
        catch (BusinessRuleViolationException)
        {
            subjectName = null;
            return false;
        }
    }

    /// <summary>
    /// Implicit conversion from SubjectName to string
    /// </summary>
    public static implicit operator string(SubjectName subjectName)
    {
        return subjectName.Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
