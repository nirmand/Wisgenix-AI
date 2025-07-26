using Wisgenix.SharedKernel.Domain;
using Wisgenix.SharedKernel.Domain.Exceptions;
using Wisgenix.SharedKernel.Domain.Enums;
using Content.Domain.Events;

namespace Content.Domain.Entities;

/// <summary>
/// Question entity representing a question within a topic
/// </summary>
public class Question : AuditableEntity
{
    private readonly List<QuestionOption> _options = new();

    public string QuestionText { get; private set; } = string.Empty;
    public int TopicId { get; private set; }
    public int DifficultyLevel { get; private set; }
    public int MaxScore { get; private set; }
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
        SetQuestionText(questionText);
        TopicId = topicId;
        SetDifficultyLevel(difficultyLevel);
        SetMaxScore(maxScore);
        GeneratedBy = generatedBy;
        SetQuestionSourceReference(questionSourceReference);
        
        AddDomainEvent(new QuestionCreatedEvent(Id, questionText, topicId));
    }

    public void UpdateQuestion(string questionText, int difficultyLevel, int maxScore, 
        QuestionSource generatedBy, string? questionSourceReference = null)
    {
        SetQuestionText(questionText);
        SetDifficultyLevel(difficultyLevel);
        SetMaxScore(maxScore);
        GeneratedBy = generatedBy;
        SetQuestionSourceReference(questionSourceReference);
        
        AddDomainEvent(new QuestionUpdatedEvent(Id, questionText));
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
        AddDomainEvent(new QuestionOptionRemovedEvent(Id, optionId, option.OptionText));
    }

    public void ValidateHasCorrectAnswer()
    {
        if (!_options.Any(o => o.IsCorrect))
        {
            throw new BusinessRuleViolationException("Question must have at least one correct answer");
        }
    }

    private void SetQuestionText(string questionText)
    {
        if (string.IsNullOrWhiteSpace(questionText))
        {
            throw new BusinessRuleViolationException("Question text cannot be empty");
        }

        if (questionText.Length > 1000)
        {
            throw new BusinessRuleViolationException("Question text cannot exceed 1000 characters");
        }

        QuestionText = questionText.Trim();
    }

    private void SetDifficultyLevel(int difficultyLevel)
    {
        if (difficultyLevel < 1 || difficultyLevel > 5)
        {
            throw new BusinessRuleViolationException("Difficulty level must be between 1 and 5");
        }

        DifficultyLevel = difficultyLevel;
    }

    private void SetMaxScore(int maxScore)
    {
        if (maxScore < 1 || maxScore > 10)
        {
            throw new BusinessRuleViolationException("Max score must be between 1 and 10");
        }

        MaxScore = maxScore;
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
