using Wisgenix.SharedKernel.Domain;
using Wisgenix.SharedKernel.Domain.Exceptions;
using Content.Domain.Events;

namespace Content.Domain.Entities;

/// <summary>
/// QuestionOption entity representing an answer option for a question
/// </summary>
public class QuestionOption : AuditableEntity
{
    public string OptionText { get; private set; } = string.Empty;
    public int QuestionId { get; private set; }
    public bool IsCorrect { get; private set; }

    // Navigation property
    public Question? Question { get; private set; }

    // Private constructor for EF Core
    private QuestionOption() { }

    public QuestionOption(string optionText, int questionId, bool isCorrect)
    {
        SetOptionText(optionText);
        QuestionId = questionId;
        IsCorrect = isCorrect;
        
        AddDomainEvent(new QuestionOptionCreatedEvent(Id, optionText, questionId, isCorrect));
    }

    public void UpdateOption(string optionText, bool isCorrect)
    {
        SetOptionText(optionText);
        IsCorrect = isCorrect;
        
        AddDomainEvent(new QuestionOptionUpdatedEvent(Id, optionText, isCorrect));
    }

    private void SetOptionText(string optionText)
    {
        if (string.IsNullOrWhiteSpace(optionText))
        {
            throw new BusinessRuleViolationException("Option text cannot be empty");
        }

        if (optionText.Length > 4000)
        {
            throw new BusinessRuleViolationException("Option text cannot exceed 4000 characters");
        }

        OptionText = optionText.Trim();
    }
}
