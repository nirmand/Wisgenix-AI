using System;
using System.Linq;
using FluentValidation;
using AIUpskillingPlatform.Core.Logger;
using AIUpskillingPlatform.DTO;

namespace AIUpskillingPlatform.DTO.Validators;

public class TopicDtoWriteValidator<T>: AbstractValidator<WriteTopicBaseDto>
{
    public TopicDtoWriteValidator()
    {
        RuleFor(x => x.TopicName)
            .NotEmpty()
            .WithMessage("Topic name is required")
            .MaximumLength(100)
            .WithMessage("Topic name must not exceed 100 characters")
            .Must(BeValidTopicName)
            .WithMessage("Topic name contains invalid characters");

        RuleFor(x => x.SubjectID)
            .GreaterThan(0)
            .WithMessage("Subject ID must be greater than 0");
    }

    private bool BeValidTopicName(string topicName)
    {
        // Add your custom validation logic here
        return !string.IsNullOrWhiteSpace(topicName) && 
               topicName.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));
    }
}

public class CreateTopicDtoValidator : AbstractValidator<CreateTopicDto>
{
    public CreateTopicDtoValidator(): base()
    {
    }
}
