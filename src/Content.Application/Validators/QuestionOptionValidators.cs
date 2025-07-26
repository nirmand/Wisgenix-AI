using FluentValidation;
using Content.Application.DTOs;

namespace Content.Application.Validators;

/// <summary>
/// Validator interface for AddQuestionOptionRequest
/// </summary>
public interface IAddQuestionOptionRequestValidator : IValidator<AddQuestionOptionRequest>
{
}

/// <summary>
/// Validator interface for UpdateQuestionOptionRequest
/// </summary>
public interface IUpdateQuestionOptionRequestValidator : IValidator<UpdateQuestionOptionRequest>
{
}

/// <summary>
/// FluentValidation implementation for AddQuestionOptionRequest
/// </summary>
public class AddQuestionOptionRequestValidator : AbstractValidator<AddQuestionOptionRequest>, IAddQuestionOptionRequestValidator
{
    public AddQuestionOptionRequestValidator()
    {
        RuleFor(x => x.OptionText)
            .NotEmpty()
            .WithMessage("Option text is required")
            .MaximumLength(4000)
            .WithMessage("Option text cannot exceed 4000 characters");

        RuleFor(x => x.QuestionId)
            .GreaterThan(0)
            .WithMessage("Question ID must be greater than 0");
    }

    async Task<ValidationResult> IValidator<AddQuestionOptionRequest>.ValidateAsync(AddQuestionOptionRequest instance, CancellationToken cancellationToken)
    {
        var result = await base.ValidateAsync(instance, cancellationToken);
        var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
        return result.IsValid ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
}

/// <summary>
/// FluentValidation implementation for UpdateQuestionOptionRequest
/// </summary>
public class UpdateQuestionOptionRequestValidator : AbstractValidator<UpdateQuestionOptionRequest>, IUpdateQuestionOptionRequestValidator
{
    public UpdateQuestionOptionRequestValidator()
    {
        RuleFor(x => x.OptionText)
            .NotEmpty()
            .WithMessage("Option text is required")
            .MaximumLength(4000)
            .WithMessage("Option text cannot exceed 4000 characters");

        RuleFor(x => x.QuestionId)
            .GreaterThan(0)
            .WithMessage("Question ID must be greater than 0");
    }

    async Task<ValidationResult> IValidator<UpdateQuestionOptionRequest>.ValidateAsync(UpdateQuestionOptionRequest instance, CancellationToken cancellationToken)
    {
        var result = await base.ValidateAsync(instance, cancellationToken);
        var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
        return result.IsValid ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
}
