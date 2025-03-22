using System;
using System.ComponentModel.DataAnnotations;

namespace AIUpskillingPlatform.Data.Entities;

public class Topic
{
    [Key]
    public int ID { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;  // Example: ".NET 8", "SQL Server"    
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
