namespace Content.Application.DTOs;

/// <summary>
/// Request DTO for adding a new question option
/// </summary>
public class AddQuestionOptionRequest
{
    public string OptionText { get; set; } = string.Empty;
    public int QuestionId { get; set; }
    public bool IsCorrect { get; set; }
}

/// <summary>
/// Request DTO for updating an existing question option
/// </summary>
public class UpdateQuestionOptionRequest
{
    public string OptionText { get; set; } = string.Empty;
    public int QuestionId { get; set; }
    public bool IsCorrect { get; set; }
}

/// <summary>
/// Response DTO for question option queries
/// </summary>
public class GetQuestionOptionResponse
{
    public int Id { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public int QuestionId { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}

/// <summary>
/// Response DTO for question options list queries
/// </summary>
public class GetQuestionOptionsResponse
{
    public IEnumerable<GetQuestionOptionResponse> Options { get; set; } = Enumerable.Empty<GetQuestionOptionResponse>();
    public int TotalCount { get; set; }
}
