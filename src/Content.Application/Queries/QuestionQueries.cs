using Wisgenix.SharedKernel.Application;
using Content.Application.DTOs;

namespace Content.Application.Queries;

/// <summary>
/// Query to get all questions
/// </summary>
public class GetAllQuestionsQuery : IQuery<GetQuestionsResponse>
{
    public bool IncludeOptions { get; }

    public GetAllQuestionsQuery(bool includeOptions = false)
    {
        IncludeOptions = includeOptions;
    }
}

/// <summary>
/// Query to get a question by ID
/// </summary>
public class GetQuestionByIdQuery : IQuery<GetQuestionResponse?>
{
    public int Id { get; }
    public bool IncludeOptions { get; }

    public GetQuestionByIdQuery(int id, bool includeOptions = false)
    {
        Id = id;
        IncludeOptions = includeOptions;
    }
}

/// <summary>
/// Query to get questions by topic ID
/// </summary>
public class GetQuestionsByTopicIdQuery : IQuery<GetQuestionsResponse>
{
    public int TopicId { get; }
    public bool IncludeOptions { get; }

    public GetQuestionsByTopicIdQuery(int topicId, bool includeOptions = false)
    {
        TopicId = topicId;
        IncludeOptions = includeOptions;
    }
}

/// <summary>
/// Query to get questions by difficulty level
/// </summary>
public class GetQuestionsByDifficultyQuery : IQuery<GetQuestionsResponse>
{
    public int DifficultyLevel { get; }
    public bool IncludeOptions { get; }

    public GetQuestionsByDifficultyQuery(int difficultyLevel, bool includeOptions = false)
    {
        DifficultyLevel = difficultyLevel;
        IncludeOptions = includeOptions;
    }
}

/// <summary>
/// Query to get a question by text and topic
/// </summary>
public class GetQuestionByTextAndTopicQuery : IQuery<GetQuestionResponse?>
{
    public string QuestionText { get; }
    public int TopicId { get; }
    public bool IncludeOptions { get; }

    public GetQuestionByTextAndTopicQuery(string questionText, int topicId, bool includeOptions = false)
    {
        QuestionText = questionText;
        TopicId = topicId;
        IncludeOptions = includeOptions;
    }
}
