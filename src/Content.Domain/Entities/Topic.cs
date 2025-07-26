using Wisgenix.SharedKernel.Domain;
using Wisgenix.SharedKernel.Domain.Exceptions;
using Content.Domain.Events;

namespace Content.Domain.Entities;

/// <summary>
/// Topic entity representing a learning topic within a subject
/// </summary>
public class Topic : AuditableEntity
{
    private readonly List<Question> _questions = new();

    public string TopicName { get; private set; } = string.Empty;
    public int SubjectId { get; private set; }
    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    // Navigation property
    public Subject? Subject { get; private set; }

    // Private constructor for EF Core
    private Topic() { }

    public Topic(string topicName, int subjectId)
    {
        SetTopicName(topicName);
        SubjectId = subjectId;
        AddDomainEvent(new TopicCreatedEvent(Id, topicName, subjectId));
    }

    public void UpdateTopicName(string topicName)
    {
        var oldName = TopicName;
        SetTopicName(topicName);
        AddDomainEvent(new TopicUpdatedEvent(Id, oldName, topicName));
    }

    public Question AddQuestion(string questionText, int difficultyLevel, int maxScore, 
        Wisgenix.SharedKernel.Domain.Enums.QuestionSource generatedBy, string? questionSourceReference = null)
    {
        if (_questions.Any(q => q.QuestionText.Equals(questionText, StringComparison.OrdinalIgnoreCase)))
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
        AddDomainEvent(new QuestionRemovedFromTopicEvent(Id, questionId, question.QuestionText));
    }

    private void SetTopicName(string topicName)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            throw new BusinessRuleViolationException("Topic name cannot be empty");
        }

        if (topicName.Length > 200)
        {
            throw new BusinessRuleViolationException("Topic name cannot exceed 200 characters");
        }

        if (ContainsInvalidCharacters(topicName))
        {
            throw new BusinessRuleViolationException("Topic name contains invalid characters");
        }

        TopicName = topicName.Trim();
    }

    private static bool ContainsInvalidCharacters(string input)
    {
        char[] invalidChars = { '>', '<', '&', '"', '\'' };
        return input.Any(c => invalidChars.Contains(c));
    }
}
