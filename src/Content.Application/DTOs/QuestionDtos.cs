using Wisgenix.SharedKernel.Domain.Enums;

namespace Content.Application.DTOs;

/// <summary>
/// Request DTO for adding a new question
/// </summary>
public class AddQuestionRequest
{
    public string QuestionText { get; set; } = string.Empty;
    public int TopicId { get; set; }
    public int DifficultyLevel { get; set; }
    public int MaxScore { get; set; }
    public QuestionSource GeneratedBy { get; set; }
    public string? QuestionSourceReference { get; set; }
}

/// <summary>
/// Request DTO for updating an existing question
/// </summary>
public class UpdateQuestionRequest
{
    public string QuestionText { get; set; } = string.Empty;
    public int TopicId { get; set; }
    public int DifficultyLevel { get; set; }
    public int MaxScore { get; set; }
    public QuestionSource GeneratedBy { get; set; }
    public string? QuestionSourceReference { get; set; }
}

/// <summary>
/// Response DTO for question queries
/// </summary>
public class GetQuestionResponse
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int DifficultyLevel { get; set; }
    public int MaxScore { get; set; }
    public QuestionSource GeneratedBy { get; set; }
    public string? QuestionSourceReference { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public ICollection<GetQuestionOptionResponse> Options { get; set; } = new List<GetQuestionOptionResponse>();
}

/// <summary>
/// Response DTO for questions list queries
/// </summary>
public class GetQuestionsResponse
{
    public IEnumerable<GetQuestionResponse> Questions { get; set; } = Enumerable.Empty<GetQuestionResponse>();
    public int TotalCount { get; set; }
}
