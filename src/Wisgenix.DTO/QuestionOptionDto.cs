using System.ComponentModel.DataAnnotations;

namespace Wisgenix.DTO;

public abstract class WriteQuestionOptionBaseDto
{
    [Required]
    public int QuestionID { get; set; }

    [Required]
    [MaxLength(4000)]
    public string OptionText { get; set; } = string.Empty;

    [Required]
    public bool IsCorrect { get; set; }
}

public class CreateQuestionOptionDto : WriteQuestionOptionBaseDto
{
}

public class UpdateQuestionOptionDto : WriteQuestionOptionBaseDto
{
}

public class QuestionOptionDto
{
    public int ID { get; set; }
    public int QuestionID { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string QuestionText { get; set; } = string.Empty;
}