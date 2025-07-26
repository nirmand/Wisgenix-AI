using Wisgenix.SharedKernel.Application;
using Wisgenix.SharedKernel.Domain.Enums;
using Content.Application.DTOs;

namespace Content.Application.Commands;

/// <summary>
/// Command to create a new question
/// </summary>
public class CreateQuestionCommand : ICommand<GetQuestionResponse>
{
    public string QuestionText { get; }
    public int TopicId { get; }
    public int DifficultyLevel { get; }
    public int MaxScore { get; }
    public QuestionSource GeneratedBy { get; }
    public string? QuestionSourceReference { get; }

    public CreateQuestionCommand(string questionText, int topicId, int difficultyLevel, 
        int maxScore, QuestionSource generatedBy, string? questionSourceReference = null)
    {
        QuestionText = questionText;
        TopicId = topicId;
        DifficultyLevel = difficultyLevel;
        MaxScore = maxScore;
        GeneratedBy = generatedBy;
        QuestionSourceReference = questionSourceReference;
    }
}

/// <summary>
/// Command to update an existing question
/// </summary>
public class UpdateQuestionCommand : ICommand<GetQuestionResponse>
{
    public int Id { get; }
    public string QuestionText { get; }
    public int TopicId { get; }
    public int DifficultyLevel { get; }
    public int MaxScore { get; }
    public QuestionSource GeneratedBy { get; }
    public string? QuestionSourceReference { get; }

    public UpdateQuestionCommand(int id, string questionText, int topicId, int difficultyLevel, 
        int maxScore, QuestionSource generatedBy, string? questionSourceReference = null)
    {
        Id = id;
        QuestionText = questionText;
        TopicId = topicId;
        DifficultyLevel = difficultyLevel;
        MaxScore = maxScore;
        GeneratedBy = generatedBy;
        QuestionSourceReference = questionSourceReference;
    }
}

/// <summary>
/// Command to delete a question
/// </summary>
public class DeleteQuestionCommand : ICommand
{
    public int Id { get; }

    public DeleteQuestionCommand(int id)
    {
        Id = id;
    }
}
