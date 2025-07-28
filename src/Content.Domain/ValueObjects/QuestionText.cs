using Wisgenix.SharedKernel.Exceptions;

namespace Content.Domain.ValueObjects;

/// <summary>
/// Value object representing question text with validation rules
/// </summary>
public class QuestionText : ValueObject
{
    public const int MaxLength = 1000;

    public string Value { get; private set; }

    private QuestionText(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new QuestionText with validation
    /// </summary>
    /// <param name="value">The question text value</param>
    /// <returns>A valid QuestionText instance</returns>
    /// <exception cref="DomainValidationException">Thrown when validation fails</exception>
    public static QuestionText Create(string value)
    {
        ValidateQuestionText(value);
        return new QuestionText(value.Trim());
    }

    /// <summary>
    /// Validates a question text value
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <exception cref="DomainValidationException">Thrown when validation fails</exception>
    private static void ValidateQuestionText(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("Question text cannot be empty");
        }

        if (value.Length > MaxLength)
        {
            throw new DomainValidationException("Question text cannot exceed {0} characters", MaxLength);
        }
    }

    /// <summary>
    /// Tries to create a QuestionText without throwing exceptions
    /// </summary>
    /// <param name="value">The question text value</param>
    /// <param name="questionText">The created QuestionText if successful</param>
    /// <returns>True if creation was successful, false otherwise</returns>
    public static bool TryCreate(string value, out QuestionText? questionText)
    {
        try
        {
            questionText = Create(value);
            return true;
        }
        catch (DomainValidationException)
        {
            questionText = null;
            return false;
        }
    }

    /// <summary>
    /// Implicit conversion from QuestionText to string
    /// </summary>
    public static implicit operator string(QuestionText questionText)
    {
        return questionText.Value;
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
