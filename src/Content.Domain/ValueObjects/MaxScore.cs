using Wisgenix.SharedKernel.Exceptions;

namespace Content.Domain.ValueObjects;

/// <summary>
/// Value object representing maximum score with validation rules
/// </summary>
public class MaxScore : ValueObject
{
    public const int MinValue = 1;
    public const int MaxValue = 10;

    public int Value { get; private set; }

    private MaxScore(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new MaxScore with validation
    /// </summary>
    /// <param name="value">The maximum score value</param>
    /// <returns>A valid MaxScore instance</returns>
    /// <exception cref="DomainValidationException">Thrown when validation fails</exception>
    public static MaxScore Create(int value)
    {
        ValidateMaxScore(value);
        return new MaxScore(value);
    }

    /// <summary>
    /// Validates a maximum score value
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <exception cref="DomainValidationException">Thrown when validation fails</exception>
    private static void ValidateMaxScore(int value)
    {
        if (value < MinValue || value > MaxValue)
        {
            throw new DomainValidationException("Max score must be between {0} and {1}", MinValue, MaxValue);
        }
    }

    /// <summary>
    /// Tries to create a MaxScore without throwing exceptions
    /// </summary>
    /// <param name="value">The maximum score value</param>
    /// <param name="maxScore">The created MaxScore if successful</param>
    /// <returns>True if creation was successful, false otherwise</returns>
    public static bool TryCreate(int value, out MaxScore? maxScore)
    {
        try
        {
            maxScore = Create(value);
            return true;
        }
        catch (DomainValidationException)
        {
            maxScore = null;
            return false;
        }
    }

    /// <summary>
    /// Implicit conversion from MaxScore to int
    /// </summary>
    public static implicit operator int(MaxScore maxScore)
    {
        return maxScore.Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
