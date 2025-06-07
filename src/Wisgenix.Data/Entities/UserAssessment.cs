using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Wisgenix.Common;
namespace Wisgenix.Data.Entities;

public class UserAssessment
{
    [Key]
    public int ID { get; set; }
    [Required]
    public int UserID { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; } // Nullable for ongoing assessments

    public double Score { get; set; } = 0; // Total score obtained by user
    public AssessmentStatus Status { get; set; } = AssessmentStatus.InProgress;   
    public ICollection<UserAssessmentQuestion> UserAssessmentQuestions { get; set; } = new List<UserAssessmentQuestion>();
    [ForeignKey("UserID")]
    public User User { get; set; } // Foreign Key
}