using FluentValidation;

namespace Wisgenix.DTO.Validators;

public class QuestionDtoWriteValidator : AbstractValidator<WriteQuestionBaseDto>
{
    public QuestionDtoWriteValidator()
    {
        RuleFor(x => x.QuestionText)
            .NotEmpty()
            .WithMessage("Question text is required")
            .MaximumLength(1000)
            .WithMessage("Question text cannot exceed 1000 characters");

        RuleFor(x => x.TopicID)
            .GreaterThan(0)
            .WithMessage("Topic ID must be greater than 0");

        RuleFor(x => x.DifficultyLevel)
            .InclusiveBetween(1, 5)
            .WithMessage("Difficulty level must be between 1 and 5");

        RuleFor(x => x.MaxScore)
            .InclusiveBetween(1, 10)
            .WithMessage("Max score must be between 1 and 10");

        RuleFor(x => x.QuestionSourceReference)
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.QuestionSourceReference))
            .WithMessage("Question source reference must be a valid URL");
    }
}

public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
{
    public CreateQuestionDtoValidator()
    {
        Include(new QuestionDtoWriteValidator());
    }
}

public class UpdateQuestionDtoValidator : AbstractValidator<UpdateQuestionDto>
{
    public UpdateQuestionDtoValidator()
    {
        Include(new QuestionDtoWriteValidator());
    }
}