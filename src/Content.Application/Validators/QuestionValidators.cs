using FluentValidation;
using Content.Application.DTOs;
using Content.Domain.ValueObjects;

namespace Content.Application.Validators;

/// <summary>
/// Validator interface for AddQuestionRequest
/// </summary>
public interface IAddQuestionRequestValidator : IValidator<AddQuestionRequest>
{
}

/// <summary>
/// Validator interface for UpdateQuestionRequest
/// </summary>
public interface IUpdateQuestionRequestValidator : IValidator<UpdateQuestionRequest>
{
}

/// <summary>
/// FluentValidation implementation for AddQuestionRequest
/// </summary>
public class AddQuestionRequestValidator : AbstractValidator<AddQuestionRequest>, IAddQuestionRequestValidator
{
    public AddQuestionRequestValidator()
    {
        RuleFor(x => x.QuestionText)
            .Must(text => !string.IsNullOrWhiteSpace(text))
            .WithMessage("Question text is required")
            .Must(text => QuestionText.TryCreate(text, out _))
            .WithMessage("Question text cannot exceed 1000 characters");

        RuleFor(x => x.TopicId)
            .GreaterThan(0)
            .WithMessage("Topic ID must be greater than 0");

        RuleFor(x => x.DifficultyLevel)
            .Must(level => DifficultyLevel.TryCreate(level, out _))
            .WithMessage("Difficulty level must be between 1 and 5");

        RuleFor(x => x.MaxScore)
            .Must(score => MaxScore.TryCreate(score, out _))
            .WithMessage("Max score must be between 1 and 10");

        RuleFor(x => x.QuestionSourceReference)
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.QuestionSourceReference))
            .WithMessage("Question source reference must be a valid URL");
    }

    async Task<ValidationResult> IValidator<AddQuestionRequest>.ValidateAsync(AddQuestionRequest instance, CancellationToken cancellationToken)
    {
        var result = await base.ValidateAsync(instance, cancellationToken);
        var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
        return result.IsValid ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
}

/// <summary>
/// FluentValidation implementation for UpdateQuestionRequest
/// </summary>
public class UpdateQuestionRequestValidator : AbstractValidator<UpdateQuestionRequest>, IUpdateQuestionRequestValidator
{
    public UpdateQuestionRequestValidator()
    {
        RuleFor(x => x.QuestionText)
            .Must(text => !string.IsNullOrWhiteSpace(text))
            .WithMessage("Question text is required")
            .Must(text => QuestionText.TryCreate(text, out _))
            .WithMessage("Question text cannot exceed 1000 characters");

        RuleFor(x => x.TopicId)
            .GreaterThan(0)
            .WithMessage("Topic ID must be greater than 0");

        RuleFor(x => x.DifficultyLevel)
            .Must(level => DifficultyLevel.TryCreate(level, out _))
            .WithMessage("Difficulty level must be between 1 and 5");

        RuleFor(x => x.MaxScore)
            .Must(score => MaxScore.TryCreate(score, out _))
            .WithMessage("Max score must be between 1 and 10");

        RuleFor(x => x.QuestionSourceReference)
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.QuestionSourceReference))
            .WithMessage("Question source reference must be a valid URL");
    }

    async Task<ValidationResult> IValidator<UpdateQuestionRequest>.ValidateAsync(UpdateQuestionRequest instance, CancellationToken cancellationToken)
    {
        var result = await base.ValidateAsync(instance, cancellationToken);
        var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
        return result.IsValid ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
}
