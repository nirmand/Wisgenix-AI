using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIUpskillingPlatform.Data.Entities;

public class UserResponse
{
    [Key]
    public int ID { get; set; }
    public int UserAssessmentQuestionID { get; set; }
    [ForeignKey("UserAssessmentQuestionID")]
    public UserAssessmentQuestion UserAssessmentQuestion { get; set; }
    public int SelectedOptionID { get; set; }
    [ForeignKey("SelectedOptionID")]
    public QuestionOption SelectedOption { get; set; }
    public int? ObtainedScore {set;get;}
    public DateTime ResponseTime { get; set; } = DateTime.UtcNow;
}