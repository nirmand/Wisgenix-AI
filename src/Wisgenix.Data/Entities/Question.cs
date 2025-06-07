using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wisgenix.Common;
namespace Wisgenix.Data.Entities;

public class Question
{
    [Key]
    public int ID { get; set; }
    [Required]
    public string QuestionText { get; set; } = string.Empty;
    [Required]
    public int TopicID { get; set; }    
    [Required]
    public int DifficultyLevel { get; set; } = 1;// 1 to 5
    public int MaxScore { get; set; } = 1; // Max score for this question (default 1)
    [Required]
    public  QuestionSource GeneratedBy { get; set; } = QuestionSource.AI;
    public string QuestionSourceReference {get;set;} // URL used to generate this question

    [ForeignKey("TopicID")]
    public Topic Topic { get; set; } // Foreign Key
    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
}
