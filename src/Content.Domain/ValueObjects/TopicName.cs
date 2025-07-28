using Wisgenix.SharedKernel.Domain.Exceptions;

namespace Content.Domain.ValueObjects;

/// <summary>
/// Value object representing a topic name with validation rules
/// </summary>
public class TopicName : ValueObject
{
    public const int MaxLength = 200;
    private static readonly char[] InvalidCharacters = { '>', '<', '&', '"', '\'' };

    public string Value { get; private set; }

    private TopicName(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new TopicName with validation
    /// </summary>
    /// <param name="value">The topic name value</param>
    /// <returns>A valid TopicName instance</returns>
    /// <exception cref="BusinessRuleViolationException">Thrown when validation fails</exception>
    public static TopicName Create(string value)
    {
        ValidateTopicName(value);
        return new TopicName(value.Trim());
    }

    /// <summary>
    /// Validates a topic name value
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <exception cref="BusinessRuleViolationException">Thrown when validation fails</exception>
    private static void ValidateTopicName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleViolationException("Topic name cannot be empty");
        }

        if (value.Length > MaxLength)
        {
            throw new BusinessRuleViolationException($"Topic name cannot exceed {MaxLength} characters");
        }

        if (ContainsInvalidCharacters(value))
        {
            throw new BusinessRuleViolationException("Topic name contains invalid characters");
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
    /// Tries to create a TopicName without throwing exceptions
    /// </summary>
    /// <param name="value">The topic name value</param>
    /// <param name="topicName">The created TopicName if successful</param>
    /// <returns>True if creation was successful, false otherwise</returns>
    public static bool TryCreate(string value, out TopicName? topicName)
    {
        try
        {
            topicName = Create(value);
            return true;
        }
        catch (BusinessRuleViolationException)
        {
            topicName = null;
            return false;
        }
    }

    /// <summary>
    /// Implicit conversion from TopicName to string
    /// </summary>
    public static implicit operator string(TopicName topicName)
    {
        return topicName.Value;
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
