using System;
using System.Linq;
using FluentValidation;
using AIUpskillingPlatform.Core.Logger;
using AIUpskillingPlatform.DTO;

namespace AIUpskillingPlatform.DTO.Validators;

public abstract class TopicDtoWriteValidator<T>: AbstractValidator<T> where T: WriteTopicBaseDto
{
    public TopicDtoWriteValidator()
    {
        RuleFor(x => x.TopicName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Topic name is required")
            .MaximumLength(100)
            .WithMessage("Topic name must not exceed 100 characters")
            .Must(BeValidTopicName)
            .WithMessage("Topic name contains invalid characters");

        RuleFor(x => x.SubjectID)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithMessage("Subject ID must be greater than 0");

        ConfigureValidationRules();
    }

    protected virtual void ConfigureValidationRules()
    {
        // This method can be overridden in derived classes to add additional validation rules
    }

    private bool BeValidTopicName(string topicName)
    {
        // Add your custom validation logic here
        return !string.IsNullOrWhiteSpace(topicName) && 
               topicName.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));
    }
}


public class CreateTopicDtoValidator: TopicDtoWriteValidator<CreateTopicDto>
{
    public CreateTopicDtoValidator(): base()
    {
    }
}

public class UpdateTopicDtoValidator: TopicDtoWriteValidator<UpdateTopicDto>
{
    public UpdateTopicDtoValidator(): base()
    {
    }
}