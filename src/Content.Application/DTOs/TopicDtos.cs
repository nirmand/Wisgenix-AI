namespace Content.Application.DTOs;

/// <summary>
/// Request DTO for adding a new topic
/// </summary>
public class AddTopicRequest
{
    public string TopicName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
}

/// <summary>
/// Request DTO for updating an existing topic
/// </summary>
public class UpdateTopicRequest
{
    public string TopicName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
}

/// <summary>
/// Response DTO for topic queries
/// </summary>
public class GetTopicResponse
{
    public int Id { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public ICollection<GetQuestionResponse> Questions { get; set; } = new List<GetQuestionResponse>();
}

/// <summary>
/// Response DTO for topics list queries
/// </summary>
public class GetTopicsResponse
{
    public IEnumerable<GetTopicResponse> Topics { get; set; } = Enumerable.Empty<GetTopicResponse>();
    public int TotalCount { get; set; }
}
