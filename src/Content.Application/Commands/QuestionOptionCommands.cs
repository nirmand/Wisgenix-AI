using Wisgenix.SharedKernel.Application;
using Content.Application.DTOs;

namespace Content.Application.Commands;

/// <summary>
/// Command to create a new question option
/// </summary>
public class CreateQuestionOptionCommand : ICommand<GetQuestionOptionResponse>
{
    public string OptionText { get; }
    public int QuestionId { get; }
    public bool IsCorrect { get; }

    public CreateQuestionOptionCommand(string optionText, int questionId, bool isCorrect)
    {
        OptionText = optionText;
        QuestionId = questionId;
        IsCorrect = isCorrect;
    }
}

/// <summary>
/// Command to update an existing question option
/// </summary>
public class UpdateQuestionOptionCommand : ICommand<GetQuestionOptionResponse>
{
    public int Id { get; }
    public string OptionText { get; }
    public int QuestionId { get; }
    public bool IsCorrect { get; }

    public UpdateQuestionOptionCommand(int id, string optionText, int questionId, bool isCorrect)
    {
        Id = id;
        OptionText = optionText;
        QuestionId = questionId;
        IsCorrect = isCorrect;
    }
}

/// <summary>
/// Command to delete a question option
/// </summary>
public class DeleteQuestionOptionCommand : ICommand
{
    public int Id { get; }

    public DeleteQuestionOptionCommand(int id)
    {
        Id = id;
    }
}
