namespace Content.Application.DTOs;

/// <summary>
/// Request DTO for adding a new subject
/// </summary>
public class AddSubjectRequest
{
    public string SubjectName { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for updating an existing subject
/// </summary>
public class UpdateSubjectRequest
{
    public string SubjectName { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for subject queries
/// </summary>
public class GetSubjectResponse
{
    public int Id { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public ICollection<GetTopicResponse> Topics { get; set; } = new List<GetTopicResponse>();
}

/// <summary>
/// Response DTO for subjects list queries
/// </summary>
public class GetSubjectsResponse
{
    public IEnumerable<GetSubjectResponse> Subjects { get; set; } = Enumerable.Empty<GetSubjectResponse>();
    public int TotalCount { get; set; }
}
