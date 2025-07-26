using FluentValidation;
using Content.Application.DTOs;

namespace Content.Application.Validators;

/// <summary>
/// Validator interface for AddSubjectRequest
/// </summary>
public interface IAddSubjectRequestValidator : IValidator<AddSubjectRequest>
{
}

/// <summary>
/// Validator interface for UpdateSubjectRequest
/// </summary>
public interface IUpdateSubjectRequestValidator : IValidator<UpdateSubjectRequest>
{
}

/// <summary>
/// FluentValidation implementation for AddSubjectRequest
/// </summary>
public class AddSubjectRequestValidator : AbstractValidator<AddSubjectRequest>, IAddSubjectRequestValidator
{
    public AddSubjectRequestValidator()
    {
        RuleFor(x => x.SubjectName)
            .NotEmpty()
            .WithMessage("Subject name is required")
            .MaximumLength(200)
            .WithMessage("Subject name must not exceed 200 characters")
            .Must(BeValidSubjectName)
            .WithMessage("Subject name contains invalid characters");
    }

    private static bool BeValidSubjectName(string subjectName)
    {
        if (string.IsNullOrWhiteSpace(subjectName))
            return false;

        char[] invalidChars = { '>', '<', '&', '"', '\'' };
        return !subjectName.Any(c => invalidChars.Contains(c));
    }

    async Task<ValidationResult> IValidator<AddSubjectRequest>.ValidateAsync(AddSubjectRequest instance, CancellationToken cancellationToken)
    {
        var result = await base.ValidateAsync(instance, cancellationToken);
        var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
        return result.IsValid ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
}

/// <summary>
/// FluentValidation implementation for UpdateSubjectRequest
/// </summary>
public class UpdateSubjectRequestValidator : AbstractValidator<UpdateSubjectRequest>, IUpdateSubjectRequestValidator
{
    public UpdateSubjectRequestValidator()
    {
        RuleFor(x => x.SubjectName)
            .NotEmpty()
            .WithMessage("Subject name is required")
            .MaximumLength(200)
            .WithMessage("Subject name must not exceed 200 characters")
            .Must(BeValidSubjectName)
            .WithMessage("Subject name contains invalid characters");
    }

    private static bool BeValidSubjectName(string subjectName)
    {
        if (string.IsNullOrWhiteSpace(subjectName))
            return false;

        char[] invalidChars = { '>', '<', '&', '"', '\'' };
        return !subjectName.Any(c => invalidChars.Contains(c));
    }

    async Task<ValidationResult> IValidator<UpdateSubjectRequest>.ValidateAsync(UpdateSubjectRequest instance, CancellationToken cancellationToken)
    {
        var result = await base.ValidateAsync(instance, cancellationToken);
        var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
        return result.IsValid ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
}
