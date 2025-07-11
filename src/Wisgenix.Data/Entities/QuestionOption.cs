using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wisgenix.Data.Entities.Base;

namespace Wisgenix.Data.Entities;

public class QuestionOption: AuditableEntity
{
    [Key]
    public int ID { get; set; }
    [Required]
    public int QuestionID { get; set; }
    [ForeignKey("QuestionID")]
    public Question Question { get; set; }
    [Required]
    public string OptionText { get; set; }
    [Required]
    public bool IsCorrect { get; set; } // Multiple correct answers allowed
}
