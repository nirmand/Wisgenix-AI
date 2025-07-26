using Wisgenix.SharedKernel.Application;
using Content.Application.DTOs;

namespace Content.Application.Queries;

/// <summary>
/// Query to get all topics
/// </summary>
public class GetAllTopicsQuery : IQuery<GetTopicsResponse>
{
    public bool IncludeQuestions { get; }

    public GetAllTopicsQuery(bool includeQuestions = false)
    {
        IncludeQuestions = includeQuestions;
    }
}

/// <summary>
/// Query to get a topic by ID
/// </summary>
public class GetTopicByIdQuery : IQuery<GetTopicResponse?>
{
    public int Id { get; }
    public bool IncludeQuestions { get; }

    public GetTopicByIdQuery(int id, bool includeQuestions = false)
    {
        Id = id;
        IncludeQuestions = includeQuestions;
    }
}

/// <summary>
/// Query to get topics by subject ID
/// </summary>
public class GetTopicsBySubjectIdQuery : IQuery<GetTopicsResponse>
{
    public int SubjectId { get; }
    public bool IncludeQuestions { get; }

    public GetTopicsBySubjectIdQuery(int subjectId, bool includeQuestions = false)
    {
        SubjectId = subjectId;
        IncludeQuestions = includeQuestions;
    }
}

/// <summary>
/// Query to get a topic by name and subject
/// </summary>
public class GetTopicByNameAndSubjectQuery : IQuery<GetTopicResponse?>
{
    public string TopicName { get; }
    public int SubjectId { get; }
    public bool IncludeQuestions { get; }

    public GetTopicByNameAndSubjectQuery(string topicName, int subjectId, bool includeQuestions = false)
    {
        TopicName = topicName;
        SubjectId = subjectId;
        IncludeQuestions = includeQuestions;
    }
}
