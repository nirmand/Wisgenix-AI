using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wisgenix.Data.Entities;

public class LearningRecommendation
{
    [Key]
    public int ID { get; set; }

    [Required]
    public int UserID { get; set; }

    // Navigation property to the User entity
    [ForeignKey("UserID")]
    public User User { get; set; }

    [Required]
    [MaxLength(200)]
    public string CourseTitle { get; set; }

    [Required]
    [MaxLength(500)]
    public string CourseURL { get; set; }

    [MaxLength(100)]
    public string Provider { get; set; } // Udemy, Microsoft Learn, PluralSight

    [Required]
    public DateTime RecommendedAt { get; set; } = DateTime.UtcNow;
}