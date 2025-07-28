using FluentValidation;
using Content.Application.DTOs;
using Content.Domain.ValueObjects;

namespace Content.Application.Validators;

/// <summary>
/// Validator interface for AddTopicRequest
/// </summary>
public interface IAddTopicRequestValidator : IValidator<AddTopicRequest>
{
}

/// <summary>
/// Validator interface for UpdateTopicRequest
/// </summary>
public interface IUpdateTopicRequestValidator : IValidator<UpdateTopicRequest>
{
}

/// <summary>
/// FluentValidation implementation for AddTopicRequest
/// </summary>
public class AddTopicRequestValidator : AbstractValidator<AddTopicRequest>, IAddTopicRequestValidator
{
    public AddTopicRequestValidator()
    {
        RuleFor(x => x.TopicName)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Topic name is required")
            .Must(name => name == null || name.Length <= TopicName.MaxLength)
            .WithMessage($"Topic name must not exceed {TopicName.MaxLength} characters")
            .Must(BeValidTopicName)
            .WithMessage("Topic name contains invalid characters");

        RuleFor(x => x.SubjectId)
            .GreaterThan(0)
            .WithMessage("Subject ID must be greater than 0");
    }

    private static bool BeValidTopicName(string topicName)
    {
        if (string.IsNullOrWhiteSpace(topicName))
            return true; // Let other rules handle null/empty

        // Only check for invalid characters
        char[] invalidChars = { '>', '<', '&', '"', '\'' };
        return !topicName.Any(c => invalidChars.Contains(c));
    }

    async Task<ValidationResult> IValidator<AddTopicRequest>.ValidateAsync(AddTopicRequest instance, CancellationToken cancellationToken)
    {
        var result = await base.ValidateAsync(instance, cancellationToken);
        var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
        return result.IsValid ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
}

/// <summary>
/// FluentValidation implementation for UpdateTopicRequest
/// </summary>
public class UpdateTopicRequestValidator : AbstractValidator<UpdateTopicRequest>, IUpdateTopicRequestValidator
{
    public UpdateTopicRequestValidator()
    {
        RuleFor(x => x.TopicName)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Topic name is required")
            .Must(name => name == null || name.Length <= TopicName.MaxLength)
            .WithMessage($"Topic name must not exceed {TopicName.MaxLength} characters")
            .Must(BeValidTopicName)
            .WithMessage("Topic name contains invalid characters");

        RuleFor(x => x.SubjectId)
            .GreaterThan(0)
            .WithMessage("Subject ID must be greater than 0");
    }

    private static bool BeValidTopicName(string topicName)
    {
        if (string.IsNullOrWhiteSpace(topicName))
            return true; // Let other rules handle null/empty

        // Only check for invalid characters
        char[] invalidChars = { '>', '<', '&', '"', '\'' };
        return !topicName.Any(c => invalidChars.Contains(c));
    }



    async Task<ValidationResult> IValidator<UpdateTopicRequest>.ValidateAsync(UpdateTopicRequest instance, CancellationToken cancellationToken)
    {
        var result = await base.ValidateAsync(instance, cancellationToken);
        var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
        return result.IsValid ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
}
