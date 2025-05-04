using FluentValidation;

namespace AIUpskillingPlatform.DTO.Validators;

public abstract class SubjectDtoWriteValidator<T>: AbstractValidator<T> where T: WriteSubjectBaseDto
{
    public SubjectDtoWriteValidator()
    {
        RuleFor(x => x.SubjectName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Subject name is required")
            .MaximumLength(200)
            .WithMessage("Subject name must not exceed 200 characters")
            .Must(BeValidSubjectName)
            .WithMessage("Subject name contains invalid characters");

        ConfigureValidationRules();
    }

    protected virtual void ConfigureValidationRules()
    {
        // This method can be overridden in derived classes to add additional validation rules
    }

    private bool BeValidSubjectName(string subjectName)
    {
        // Add your custom validation logic here
        return !string.IsNullOrWhiteSpace(subjectName) && 
               subjectName.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));
    }
}


public class CreateSubjectDtoValidator: SubjectDtoWriteValidator<CreateSubjectDto>
{
    public CreateSubjectDtoValidator(): base()
    {
    }
}

public class UpdateSubjectDtoValidator: SubjectDtoWriteValidator<UpdateSubjectDto>
{
    public UpdateSubjectDtoValidator(): base()
    {
    }
}