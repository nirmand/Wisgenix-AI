using Wisgenix.SharedKernel.Domain;

namespace Content.Domain.Events;

public class QuestionCreatedEvent : BaseDomainEvent
{
    public int QuestionId { get; }
    public string QuestionText { get; }
    public int TopicId { get; }

    public QuestionCreatedEvent(int questionId, string questionText, int topicId)
    {
        QuestionId = questionId;
        QuestionText = questionText;
        TopicId = topicId;
    }
}

public class QuestionUpdatedEvent : BaseDomainEvent
{
    public int QuestionId { get; }
    public string QuestionText { get; }

    public QuestionUpdatedEvent(int questionId, string questionText)
    {
        QuestionId = questionId;
        QuestionText = questionText;
    }
}

public class QuestionOptionAddedEvent : BaseDomainEvent
{
    public int QuestionId { get; }
    public int OptionId { get; }
    public string OptionText { get; }
    public bool IsCorrect { get; }

    public QuestionOptionAddedEvent(int questionId, int optionId, string optionText, bool isCorrect)
    {
        QuestionId = questionId;
        OptionId = optionId;
        OptionText = optionText;
        IsCorrect = isCorrect;
    }
}

public class QuestionOptionRemovedEvent : BaseDomainEvent
{
    public int QuestionId { get; }
    public int OptionId { get; }
    public string OptionText { get; }

    public QuestionOptionRemovedEvent(int questionId, int optionId, string optionText)
    {
        QuestionId = questionId;
        OptionId = optionId;
        OptionText = optionText;
    }
}

public class QuestionOptionCreatedEvent : BaseDomainEvent
{
    public int OptionId { get; }
    public string OptionText { get; }
    public int QuestionId { get; }
    public bool IsCorrect { get; }

    public QuestionOptionCreatedEvent(int optionId, string optionText, int questionId, bool isCorrect)
    {
        OptionId = optionId;
        OptionText = optionText;
        QuestionId = questionId;
        IsCorrect = isCorrect;
    }
}

public class QuestionOptionUpdatedEvent : BaseDomainEvent
{
    public int OptionId { get; }
    public string OptionText { get; }
    public bool IsCorrect { get; }

    public QuestionOptionUpdatedEvent(int optionId, string optionText, bool isCorrect)
    {
        OptionId = optionId;
        OptionText = optionText;
        IsCorrect = isCorrect;
    }
}
