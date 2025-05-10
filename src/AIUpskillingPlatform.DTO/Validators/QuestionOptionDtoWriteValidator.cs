using FluentValidation;

namespace AIUpskillingPlatform.DTO.Validators;

public class QuestionOptionDtoWriteValidator : AbstractValidator<WriteQuestionOptionBaseDto>
{
    public QuestionOptionDtoWriteValidator()
    {
        RuleFor(x => x.QuestionID)
            .GreaterThan(0)
            .WithMessage("Question ID must be greater than 0");

        RuleFor(x => x.OptionText)
            .NotEmpty()
            .WithMessage("Option text is required")
            .MaximumLength(4000)
            .WithMessage("Option text cannot exceed 4000 characters");
    }
}

public class CreateQuestionOptionDtoValidator : AbstractValidator<CreateQuestionOptionDto>
{
    public CreateQuestionOptionDtoValidator()
    {
        Include(new QuestionOptionDtoWriteValidator());
    }
}

public class UpdateQuestionOptionDtoValidator : AbstractValidator<UpdateQuestionOptionDto>
{
    public UpdateQuestionOptionDtoValidator()
    {
        Include(new QuestionOptionDtoWriteValidator());
    }
}