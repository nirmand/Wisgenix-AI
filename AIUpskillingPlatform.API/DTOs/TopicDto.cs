using System.ComponentModel.DataAnnotations;

namespace AIUpskillingPlatform.API.DTOs;

public class CreateTopicDto
{
    [Required]
    [MaxLength(200)]
    public string TopicName { get; set; } = string.Empty;
}

public class UpdateTopicDto
{
    [Required]
    [MaxLength(200)]
    public string TopicName { get; set; } = string.Empty;
}

public class TopicDto
{
    public int ID { get; set; }
    public string TopicName { get; set; } = string.Empty;
} 