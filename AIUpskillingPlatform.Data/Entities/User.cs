using System;
using System.ComponentModel.DataAnnotations;
using AIUpskillingPlatform.Common;

namespace AIUpskillingPlatform.Data.Entities;

public class User
{
    [Key]
    public int ID { get; set; }
    [Required]
    public string FullName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public ExperienceLevel ExperienceLevel { get; set; } // Junior, Mid, Senior
}