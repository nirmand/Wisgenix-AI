using Wisgenix.SharedKernel.Domain;
using Wisgenix.SharedKernel.Exceptions;
using Wisgenix.SharedKernel.Domain.Enums;
using Content.Domain.Events;
using Content.Domain.ValueObjects;

namespace Content.Domain.Entities;

/// <summary>
/// Question entity representing a question within a topic
/// </summary>
public class Question : AuditableEntity
{
    private readonly List<QuestionOption> _options = new();

    public QuestionText QuestionText { get; private set; } = QuestionText.Create("Default");
    public int TopicId { get; private set; }
    public DifficultyLevel DifficultyLevel { get; private set; } = DifficultyLevel.Create(1);
    public MaxScore MaxScore { get; private set; } = MaxScore.Create(1);
    public QuestionSource GeneratedBy { get; private set; }
    public string? QuestionSourceReference { get; private set; }
    
    public IReadOnlyCollection<QuestionOption> Options => _options.AsReadOnly();

    // Navigation property
    public Topic? Topic { get; private set; }

    // Private constructor for EF Core
    private Question() { }

    public Question(string questionText, int topicId, int difficultyLevel, int maxScore,
        QuestionSource generatedBy, string? questionSourceReference = null)
    {
        QuestionText = QuestionText.Create(questionText);
        TopicId = topicId;
        DifficultyLevel = DifficultyLevel.Create(difficultyLevel);
        MaxScore = MaxScore.Create(maxScore);
        GeneratedBy = generatedBy;
        SetQuestionSourceReference(questionSourceReference);

        AddDomainEvent(new QuestionCreatedEvent(Id, QuestionText.Value, topicId));
    }

    public void UpdateQuestion(string questionText, int difficultyLevel, int maxScore,
        QuestionSource generatedBy, string? questionSourceReference = null)
    {
        QuestionText = QuestionText.Create(questionText);
        DifficultyLevel = DifficultyLevel.Create(difficultyLevel);
        MaxScore = MaxScore.Create(maxScore);
        GeneratedBy = generatedBy;
        SetQuestionSourceReference(questionSourceReference);

        AddDomainEvent(new QuestionUpdatedEvent(Id, QuestionText.Value));
    }

    public QuestionOption AddOption(string optionText, bool isCorrect)
    {
        var option = new QuestionOption(optionText, Id, isCorrect);
        _options.Add(option);
        AddDomainEvent(new QuestionOptionAddedEvent(Id, option.Id, optionText, isCorrect));
        return option;
    }

    public void RemoveOption(int optionId)
    {
        var option = _options.FirstOrDefault(o => o.Id == optionId);
        if (option == null)
        {
            throw new EntityNotFoundException(nameof(QuestionOption), optionId);
        }

        _options.Remove(option);
        AddDomainEvent(new QuestionOptionRemovedEvent(Id, optionId, option.OptionText.Value));
    }

    public void ValidateHasCorrectAnswer()
    {
        if (!_options.Any(o => o.IsCorrect))
        {
            throw new BusinessRuleViolationException("Question must have at least one correct answer");
        }
    }



    private void SetQuestionSourceReference(string? questionSourceReference)
    {
        if (!string.IsNullOrEmpty(questionSourceReference) && 
            !Uri.TryCreate(questionSourceReference, UriKind.Absolute, out _))
        {
            throw new BusinessRuleViolationException("Question source reference must be a valid URL");
        }

        QuestionSourceReference = questionSourceReference;
    }
}
