using Wisgenix.SharedKernel.Domain;
using Wisgenix.SharedKernel.Domain.Exceptions;
using Content.Domain.Events;

namespace Content.Domain.Entities;

/// <summary>
/// Subject aggregate root representing a learning subject
/// </summary>
public class Subject : AuditableEntity
{
    private readonly List<Topic> _topics = new();

    public string SubjectName { get; private set; } = string.Empty;
    public IReadOnlyCollection<Topic> Topics => _topics.AsReadOnly();

    // Private constructor for EF Core
    private Subject() { }

    public Subject(string subjectName)
    {
        SetSubjectName(subjectName);
        AddDomainEvent(new SubjectCreatedEvent(Id, subjectName));
    }

    public void UpdateSubjectName(string subjectName)
    {
        var oldName = SubjectName;
        SetSubjectName(subjectName);
        AddDomainEvent(new SubjectUpdatedEvent(Id, oldName, subjectName));
    }

    public Topic AddTopic(string topicName)
    {
        if (_topics.Any(t => t.TopicName.Equals(topicName, StringComparison.OrdinalIgnoreCase)))
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
        AddDomainEvent(new TopicRemovedFromSubjectEvent(Id, topicId, topic.TopicName));
    }

    private void SetSubjectName(string subjectName)
    {
        if (string.IsNullOrWhiteSpace(subjectName))
        {
            throw new BusinessRuleViolationException("Subject name cannot be empty");
        }

        if (subjectName.Length > 200)
        {
            throw new BusinessRuleViolationException("Subject name cannot exceed 200 characters");
        }

        if (ContainsInvalidCharacters(subjectName))
        {
            throw new BusinessRuleViolationException("Subject name contains invalid characters");
        }

        SubjectName = subjectName.Trim();
    }

    private static bool ContainsInvalidCharacters(string input)
    {
        char[] invalidChars = { '>', '<', '&', '"', '\'' };
        return input.Any(c => invalidChars.Contains(c));
    }
}
