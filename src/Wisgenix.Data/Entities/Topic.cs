using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wisgenix.Data.Entities.Base;
namespace Wisgenix.Data.Entities;

public class Topic: AuditableEntity
{
    [Key]
    public int ID { get; set; }
    [Required]
    public string TopicName { get; set; } = string.Empty;  // Example: ".NET 8", "SQL Server"    
    public int SubjectID { get; set; }
    [ForeignKey("SubjectID")]
    public Subject Subject { get; set; } // Foreign Key
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
