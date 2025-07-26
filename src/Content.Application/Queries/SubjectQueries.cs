using Wisgenix.SharedKernel.Application;
using Content.Application.DTOs;

namespace Content.Application.Queries;

/// <summary>
/// Query to get all subjects
/// </summary>
public class GetAllSubjectsQuery : IQuery<GetSubjectsResponse>
{
    public bool IncludeTopics { get; }

    public GetAllSubjectsQuery(bool includeTopics = false)
    {
        IncludeTopics = includeTopics;
    }
}

/// <summary>
/// Query to get a subject by ID
/// </summary>
public class GetSubjectByIdQuery : IQuery<GetSubjectResponse?>
{
    public int Id { get; }
    public bool IncludeTopics { get; }

    public GetSubjectByIdQuery(int id, bool includeTopics = false)
    {
        Id = id;
        IncludeTopics = includeTopics;
    }
}

/// <summary>
/// Query to get a subject by name
/// </summary>
public class GetSubjectByNameQuery : IQuery<GetSubjectResponse?>
{
    public string SubjectName { get; }
    public bool IncludeTopics { get; }

    public GetSubjectByNameQuery(string subjectName, bool includeTopics = false)
    {
        SubjectName = subjectName;
        IncludeTopics = includeTopics;
    }
}
