using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wisgenix.Data.Entities.Base;

namespace Wisgenix.Data.Entities;

public class UserResponse: AuditableEntity
{
    [Key]
    public int ID { get; set; }
    public int UserAssessmentQuestionID { get; set; }
    [ForeignKey("UserAssessmentQuestionID")]
    public UserAssessmentQuestion UserAssessmentQuestion { get; set; }
    public int SelectedOptionID { get; set; }
    [ForeignKey("SelectedOptionID")]
    public QuestionOption SelectedOption { get; set; }
    public DateTime ResponseTime { get; set; } = DateTime.UtcNow;
}