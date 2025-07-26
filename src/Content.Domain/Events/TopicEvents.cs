using Wisgenix.SharedKernel.Domain;

namespace Content.Domain.Events;

public class TopicCreatedEvent : BaseDomainEvent
{
    public int TopicId { get; }
    public string TopicName { get; }
    public int SubjectId { get; }

    public TopicCreatedEvent(int topicId, string topicName, int subjectId)
    {
        TopicId = topicId;
        TopicName = topicName;
        SubjectId = subjectId;
    }
}

public class TopicUpdatedEvent : BaseDomainEvent
{
    public int TopicId { get; }
    public string OldName { get; }
    public string NewName { get; }

    public TopicUpdatedEvent(int topicId, string oldName, string newName)
    {
        TopicId = topicId;
        OldName = oldName;
        NewName = newName;
    }
}

public class QuestionAddedToTopicEvent : BaseDomainEvent
{
    public int TopicId { get; }
    public int QuestionId { get; }
    public string QuestionText { get; }

    public QuestionAddedToTopicEvent(int topicId, int questionId, string questionText)
    {
        TopicId = topicId;
        QuestionId = questionId;
        QuestionText = questionText;
    }
}

public class QuestionRemovedFromTopicEvent : BaseDomainEvent
{
    public int TopicId { get; }
    public int QuestionId { get; }
    public string QuestionText { get; }

    public QuestionRemovedFromTopicEvent(int topicId, int questionId, string questionText)
    {
        TopicId = topicId;
        QuestionId = questionId;
        QuestionText = questionText;
    }
}
