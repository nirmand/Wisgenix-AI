using System.ComponentModel.DataAnnotations;

namespace AIUpskillingPlatform.API.DTOs;

public class CreateTopicDto
{
    [Required(ErrorMessage = "Topic name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Topic name must be between 2 and 200 characters")]
    public string TopicName { get; set; } = String.Empty;
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