using Wisgenix.SharedKernel.Domain;

namespace Content.Domain.Events;

public class SubjectCreatedEvent : BaseDomainEvent
{
    public int SubjectId { get; }
    public string SubjectName { get; }

    public SubjectCreatedEvent(int subjectId, string subjectName)
    {
        SubjectId = subjectId;
        SubjectName = subjectName;
    }
}

public class SubjectUpdatedEvent : BaseDomainEvent
{
    public int SubjectId { get; }
    public string OldName { get; }
    public string NewName { get; }

    public SubjectUpdatedEvent(int subjectId, string oldName, string newName)
    {
        SubjectId = subjectId;
        OldName = oldName;
        NewName = newName;
    }
}

public class TopicAddedToSubjectEvent : BaseDomainEvent
{
    public int SubjectId { get; }
    public int TopicId { get; }
    public string TopicName { get; }

    public TopicAddedToSubjectEvent(int subjectId, int topicId, string topicName)
    {
        SubjectId = subjectId;
        TopicId = topicId;
        TopicName = topicName;
    }
}

public class TopicRemovedFromSubjectEvent : BaseDomainEvent
{
    public int SubjectId { get; }
    public int TopicId { get; }
    public string TopicName { get; }

    public TopicRemovedFromSubjectEvent(int subjectId, int topicId, string topicName)
    {
        SubjectId = subjectId;
        TopicId = topicId;
        TopicName = topicName;
    }
}
