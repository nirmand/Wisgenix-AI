using System.ComponentModel.DataAnnotations;

namespace AIUpskillingPlatform.API.DTOs;

public class CreateQuestionOptionDto
{
    [Required]
    public int QuestionID { get; set; }

    [Required]
    [MaxLength(4000)]
    public string OptionText { get; set; } = string.Empty;

    [Required]
    public bool IsCorrect { get; set; }
}

public class UpdateQuestionOptionDto
{
    [Required]
    public int QuestionID { get; set; }

    [Required]
    [MaxLength(4000)]
    public string OptionText { get; set; } = string.Empty;

    [Required]
    public bool IsCorrect { get; set; }
}

public class QuestionOptionDto
{
    public int ID { get; set; }
    public int QuestionID { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
} 