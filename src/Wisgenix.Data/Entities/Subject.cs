using System;
using System.ComponentModel.DataAnnotations;

namespace Wisgenix.Data.Entities;

public class Subject
{
    [Key]
    public int ID { get; set; }
    [Required]
    public string SubjectName { get; set; } = string.Empty;  // Example: ".NET 8", "SQL Server"    
    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
