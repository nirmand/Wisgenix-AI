using Wisgenix.SharedKernel.Domain;
using Wisgenix.SharedKernel.Exceptions;
using Content.Domain.Events;
using Content.Domain.ValueObjects;

namespace Content.Domain.Entities;

/// <summary>
/// QuestionOption entity representing an answer option for a question
/// </summary>
public class QuestionOption : AuditableEntity
{
    public OptionText OptionText { get; private set; } = OptionText.Create("Default");
    public int QuestionId { get; private set; }
    public bool IsCorrect { get; private set; }

    // Navigation property
    public Question? Question { get; private set; }

    // Private constructor for EF Core
    private QuestionOption() { }

    public QuestionOption(string optionText, int questionId, bool isCorrect)
    {
        OptionText = OptionText.Create(optionText);
        QuestionId = questionId;
        IsCorrect = isCorrect;

        AddDomainEvent(new QuestionOptionCreatedEvent(Id, OptionText.Value, questionId, isCorrect));
    }

    public void UpdateOption(string optionText, bool isCorrect)
    {
        OptionText = OptionText.Create(optionText);
        IsCorrect = isCorrect;

        AddDomainEvent(new QuestionOptionUpdatedEvent(Id, OptionText.Value, isCorrect));
    }


}
