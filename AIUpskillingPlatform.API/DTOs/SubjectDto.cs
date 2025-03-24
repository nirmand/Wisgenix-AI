using System.ComponentModel.DataAnnotations;

namespace AIUpskillingPlatform.API.DTOs;

public class CreateSubjectDto
{
    [Required]
    [MaxLength(100)]
    public string SubjectName { get; set; } = string.Empty;
}

public class UpdateSubjectDto
{
    [Required]
    [MaxLength(100)]
    public string SubjectName { get; set; } = string.Empty;
}

public class SubjectDto
{
    public int ID { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public ICollection<TopicDto> Topics { get; set; } = new List<TopicDto>();
} 