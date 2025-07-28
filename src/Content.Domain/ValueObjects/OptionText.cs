using Wisgenix.SharedKernel.Domain.Exceptions;

namespace Content.Domain.ValueObjects;

/// <summary>
/// Value object representing option text with validation rules
/// </summary>
public class OptionText : ValueObject
{
    public const int MaxLength = 4000;

    public string Value { get; private set; }

    private OptionText(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new OptionText with validation
    /// </summary>
    /// <param name="value">The option text value</param>
    /// <returns>A valid OptionText instance</returns>
    /// <exception cref="BusinessRuleViolationException">Thrown when validation fails</exception>
    public static OptionText Create(string value)
    {
        ValidateOptionText(value);
        return new OptionText(value.Trim());
    }

    /// <summary>
    /// Validates an option text value
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <exception cref="BusinessRuleViolationException">Thrown when validation fails</exception>
    private static void ValidateOptionText(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleViolationException("Option text cannot be empty");
        }

        if (value.Length > MaxLength)
        {
            throw new BusinessRuleViolationException($"Option text cannot exceed {MaxLength} characters");
        }
    }

    /// <summary>
    /// Tries to create an OptionText without throwing exceptions
    /// </summary>
    /// <param name="value">The option text value</param>
    /// <param name="optionText">The created OptionText if successful</param>
    /// <returns>True if creation was successful, false otherwise</returns>
    public static bool TryCreate(string value, out OptionText? optionText)
    {
        try
        {
            optionText = Create(value);
            return true;
        }
        catch (BusinessRuleViolationException)
        {
            optionText = null;
            return false;
        }
    }

    /// <summary>
    /// Implicit conversion from OptionText to string
    /// </summary>
    public static implicit operator string(OptionText optionText)
    {
        return optionText.Value;
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
