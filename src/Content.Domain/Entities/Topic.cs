using Wisgenix.SharedKernel.Domain;
using Wisgenix.SharedKernel.Domain.Exceptions;
using Content.Domain.Events;
using Content.Domain.ValueObjects;

namespace Content.Domain.Entities;

/// <summary>
/// Topic entity representing a learning topic within a subject
/// </summary>
public class Topic : AuditableEntity
{
    private readonly List<Question> _questions = new();

    public TopicName TopicName { get; private set; } = TopicName.Create("Default");
    public int SubjectId { get; private set; }
    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    // Navigation property
    public Subject? Subject { get; private set; }

    // Private constructor for EF Core
    private Topic() { }

    public Topic(string topicName, int subjectId)
    {
        TopicName = TopicName.Create(topicName);
        SubjectId = subjectId;
        AddDomainEvent(new TopicCreatedEvent(Id, TopicName.Value, subjectId));
    }

    public void UpdateTopicName(string topicName)
    {
        var oldName = TopicName.Value;
        TopicName = TopicName.Create(topicName);
        AddDomainEvent(new TopicUpdatedEvent(Id, oldName, TopicName.Value));
    }

    public Question AddQuestion(string questionText, int difficultyLevel, int maxScore, 
        Wisgenix.SharedKernel.Domain.Enums.QuestionSource generatedBy, string? questionSourceReference = null)
    {
        if (_questions.Any(q => q.QuestionText.Value.Equals(questionText, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DuplicateEntityException(nameof(Question), nameof(Question.QuestionText), questionText);
        }

        var question = new Question(questionText, Id, difficultyLevel, maxScore, generatedBy, questionSourceReference);
        _questions.Add(question);
        AddDomainEvent(new QuestionAddedToTopicEvent(Id, question.Id, questionText));
        return question;
    }

    public void RemoveQuestion(int questionId)
    {
        var question = _questions.FirstOrDefault(q => q.Id == questionId);
        if (question == null)
        {
            throw new EntityNotFoundException(nameof(Question), questionId);
        }

        _questions.Remove(question);
        AddDomainEvent(new QuestionRemovedFromTopicEvent(Id, questionId, question.QuestionText.Value));
    }


}
