using Wisgenix.SharedKernel.Domain;
using Wisgenix.SharedKernel.Domain.Exceptions;
using Content.Domain.Events;
using Content.Domain.ValueObjects;

namespace Content.Domain.Entities;

/// <summary>
/// Subject aggregate root representing a learning subject
/// </summary>
public class Subject : AuditableEntity
{
    private readonly List<Topic> _topics = new();

    public SubjectName SubjectName { get; private set; } = SubjectName.Create("Default");
    public IReadOnlyCollection<Topic> Topics => _topics.AsReadOnly();

    // Private constructor for EF Core
    private Subject() { }

    public Subject(string subjectName)
    {
        SubjectName = SubjectName.Create(subjectName);
        AddDomainEvent(new SubjectCreatedEvent(Id, SubjectName.Value));
    }

    public void UpdateSubjectName(string subjectName)
    {
        var oldName = SubjectName.Value;
        SubjectName = SubjectName.Create(subjectName);
        AddDomainEvent(new SubjectUpdatedEvent(Id, oldName, SubjectName.Value));
    }

    public Topic AddTopic(string topicName)
    {
        if (_topics.Any(t => t.TopicName.Value.Equals(topicName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DuplicateEntityException(nameof(Topic), nameof(Topic.TopicName), topicName);
        }

        var topic = new Topic(topicName, Id);
        _topics.Add(topic);
        AddDomainEvent(new TopicAddedToSubjectEvent(Id, topic.Id, topicName));
        return topic;
    }

    public void RemoveTopic(int topicId)
    {
        var topic = _topics.FirstOrDefault(t => t.Id == topicId);
        if (topic == null)
        {
            throw new EntityNotFoundException(nameof(Topic), topicId);
        }

        _topics.Remove(topic);
        AddDomainEvent(new TopicRemovedFromSubjectEvent(Id, topicId, topic.TopicName.Value));
    }


}
