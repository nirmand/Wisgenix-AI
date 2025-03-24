using System.ComponentModel.DataAnnotations;

namespace AIUpskillingPlatform.API.DTOs;

public class CreateTopicDto
{
    [Required]
    [MaxLength(100)]
    public string TopicName { get; set; } = string.Empty;

    [Required]
    public int SubjectID { get; set; }
}

public class UpdateTopicDto
{
    [Required]
    [MaxLength(100)]
    public string TopicName { get; set; } = string.Empty;

    [Required]
    public int SubjectID { get; set; }
}

public class TopicDto
{
    public int ID { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int SubjectID { get; set; }
    public string SubjectName { get; set; } = string.Empty;
}
