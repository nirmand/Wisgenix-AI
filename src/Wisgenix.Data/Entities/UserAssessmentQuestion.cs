using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Wisgenix.Data.Entities;

public class UserAssessmentQuestion
{
    public int ID { get; set; }
    public int UserAssessmentID { get; set; }
    public int QuestionID { get; set; }

    [ForeignKey("UserAssessmentID")]
    public UserAssessment UserAssessment { get; set; }
    [ForeignKey("QuestionID")]
    public Question Question { get; set; }
    public int? ObtainedScore {set;get;}
}