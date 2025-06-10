using System;
using System.ComponentModel.DataAnnotations;
using Wisgenix.Common;
using Wisgenix.Data.Entities.Base;

namespace Wisgenix.Data.Entities;

public class User: AuditableEntity
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