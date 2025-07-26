using Wisgenix.SharedKernel.Application;
using Content.Application.DTOs;

namespace Content.Application.Queries;

/// <summary>
/// Query to get all question options
/// </summary>
public class GetAllQuestionOptionsQuery : IQuery<GetQuestionOptionsResponse>
{
}

/// <summary>
/// Query to get a question option by ID
/// </summary>
public class GetQuestionOptionByIdQuery : IQuery<GetQuestionOptionResponse?>
{
    public int Id { get; }

    public GetQuestionOptionByIdQuery(int id)
    {
        Id = id;
    }
}

/// <summary>
/// Query to get question options by question ID
/// </summary>
public class GetQuestionOptionsByQuestionIdQuery : IQuery<GetQuestionOptionsResponse>
{
    public int QuestionId { get; }

    public GetQuestionOptionsByQuestionIdQuery(int questionId)
    {
        QuestionId = questionId;
    }
}

/// <summary>
/// Query to get correct options by question ID
/// </summary>
public class GetCorrectOptionsByQuestionIdQuery : IQuery<GetQuestionOptionsResponse>
{
    public int QuestionId { get; }

    public GetCorrectOptionsByQuestionIdQuery(int questionId)
    {
        QuestionId = questionId;
    }
}
