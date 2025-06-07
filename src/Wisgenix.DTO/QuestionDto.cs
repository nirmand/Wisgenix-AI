using System.ComponentModel.DataAnnotations;
using Wisgenix.Common;

namespace Wisgenix.DTO;

public abstract class WriteQuestionBaseDto
{
    [Required]
    [MaxLength(1000)]
    public string QuestionText { get; set; } = string.Empty;

    [Required]
    public int TopicID { get; set; }

    [Required]
    [Range(1, 5)]
    public int DifficultyLevel { get; set; }

    [Required]
    [Range(1, 10)]
    public int MaxScore { get; set; }

    [Required]
    public QuestionSource GeneratedBy { get; set; }

    [Url]
    public string? QuestionSourceReference { get; set; }
}

public class CreateQuestionDto : WriteQuestionBaseDto
{
}

public class UpdateQuestionDto : WriteQuestionBaseDto
{
}

public class QuestionDto
{
    public int ID { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int TopicID { get; set; }
    public int DifficultyLevel { get; set; }
    public int MaxScore { get; set; }
    public QuestionSource GeneratedBy { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public string? QuestionSourceReference { get; set; }
}