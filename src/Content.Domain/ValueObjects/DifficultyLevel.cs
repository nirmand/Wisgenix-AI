using Wisgenix.SharedKernel.Domain.Exceptions;

namespace Content.Domain.ValueObjects;

/// <summary>
/// Value object representing difficulty level with validation rules
/// </summary>
public class DifficultyLevel : ValueObject
{
    public const int MinValue = 1;
    public const int MaxValue = 5;

    public int Value { get; private set; }

    private DifficultyLevel(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new DifficultyLevel with validation
    /// </summary>
    /// <param name="value">The difficulty level value</param>
    /// <returns>A valid DifficultyLevel instance</returns>
    /// <exception cref="BusinessRuleViolationException">Thrown when validation fails</exception>
    public static DifficultyLevel Create(int value)
    {
        ValidateDifficultyLevel(value);
        return new DifficultyLevel(value);
    }

    /// <summary>
    /// Validates a difficulty level value
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <exception cref="BusinessRuleViolationException">Thrown when validation fails</exception>
    private static void ValidateDifficultyLevel(int value)
    {
        if (value < MinValue || value > MaxValue)
        {
            throw new BusinessRuleViolationException($"Difficulty level must be between {MinValue} and {MaxValue}");
        }
    }

    /// <summary>
    /// Tries to create a DifficultyLevel without throwing exceptions
    /// </summary>
    /// <param name="value">The difficulty level value</param>
    /// <param name="difficultyLevel">The created DifficultyLevel if successful</param>
    /// <returns>True if creation was successful, false otherwise</returns>
    public static bool TryCreate(int value, out DifficultyLevel? difficultyLevel)
    {
        try
        {
            difficultyLevel = Create(value);
            return true;
        }
        catch (BusinessRuleViolationException)
        {
            difficultyLevel = null;
            return false;
        }
    }

    /// <summary>
    /// Implicit conversion from DifficultyLevel to int
    /// </summary>
    public static implicit operator int(DifficultyLevel difficultyLevel)
    {
        return difficultyLevel.Value;
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
