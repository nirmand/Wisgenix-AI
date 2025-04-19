using AIUpskillingPlatform.API.DTOs;
using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AIUpskillingPlatform.Core.Logger;

namespace AIUpskillingPlatform.API.Controllers;

[ApiController]
[Route("api/content")]
public class TopicsController : ControllerBase
{
    private readonly ITopicRepository _topicRepository;
    private readonly ILoggingService _logger;

    public TopicsController(ITopicRepository topicRepository, ILoggingService logger)
    {
        _topicRepository = topicRepository;
        _logger = logger;
    }

    [HttpGet("topics")]
    public async Task<ActionResult<IEnumerable<TopicDto>>> GetTopics()
    {
        try
        {
            _logger.LogInformation("Getting all topics");
            var topics = await _topicRepository.GetAllAsync();
            var topicDtos = topics.Select(t => new TopicDto { ID = t.ID, TopicName = t.TopicName, SubjectID = t.SubjectID });  
            _logger.LogInformation($"Successfully retrieved {topics.Count()} topics");
            return Ok(topicDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all topics");
            return StatusCode(500, "An error occurred while retrieving topics");
        }
    }

    [HttpGet("get-topic/{id}")]
    public async Task<ActionResult<TopicDto>> GetTopic(int id)
    {
        try
        {
            _logger.LogInformation($"Getting topic with ID: {id}");
            var topic = await _topicRepository.GetByIdAsync(id);
            var topicDto = new TopicDto { ID = topic.ID, TopicName = topic.TopicName, SubjectID = topic.SubjectID };
            _logger.LogInformation($"Successfully retrieved topic with ID: {id}");
            return Ok(topicDto);
        }
        catch (TopicNotFoundException ex)
        {
            _logger.LogWarning($"Topic with ID: {id} was not found");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting topic with ID: {id}");
            return StatusCode(500, "An error occurred while retrieving the topic");
        }
    }

    [HttpPost("create-topic")]
    public async Task<ActionResult<TopicDto>> CreateTopic(CreateTopicDto createTopicDto)
    {
        try
        {
            if (createTopicDto.SubjectID <= 0)
            {
                return BadRequest("Subject ID is required");
            }

            if (String.IsNullOrWhiteSpace(createTopicDto.TopicName))
            {
                return BadRequest("Topic name is required");
            }

            // Check if subject exists
            if (!await _topicRepository.SubjectExistsAsync(createTopicDto.SubjectID))
            {
                return BadRequest($"Subject with ID {createTopicDto.SubjectID} does not exist");
            }

            var topic = new Topic
            {
                TopicName = createTopicDto.TopicName,
                SubjectID = createTopicDto.SubjectID
            };

            var createdTopic = await _topicRepository.CreateAsync(topic);
            
            var topicDto = new TopicDto
            {
                ID = createdTopic.ID,
                TopicName = createdTopic.TopicName,
                SubjectID = createdTopic.SubjectID
            };

            return CreatedAtAction(nameof(GetTopic), new { id = createdTopic.ID }, topicDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating topic");
            return StatusCode(500, "An error occurred while creating the topic");
        }
    }

    [HttpPut("update-topic/{id}")]
    public async Task<IActionResult> UpdateTopic(int id, UpdateTopicDto updateTopicDto)
    {
        try
        {
            // Check if subject exists
            if (!await _topicRepository.SubjectExistsAsync(updateTopicDto.SubjectID))
            {
                return BadRequest($"Subject with ID {updateTopicDto.SubjectID} does not exist");
            }

            var topic = await _topicRepository.GetByIdAsync(id);
            if (topic == null)
            {
                return NotFound($"Topic with ID {id} was not found");
            }

            topic.TopicName = updateTopicDto.TopicName;
            topic.SubjectID = updateTopicDto.SubjectID;

            await _topicRepository.UpdateAsync(topic);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating topic with ID: {id}");
            return StatusCode(500, "An error occurred while updating the topic");
        }
    }

    [HttpDelete("delete-topic/{id}")]
    public async Task<IActionResult> DeleteTopic(int id)
    {
        try
        {
            _logger.LogInformation("Deleting topic with ID: {Id}", id);
            await _topicRepository.DeleteAsync(id);
            _logger.LogInformation("Successfully deleted topic with ID: {Id}", id);
            return NoContent();
        }
        catch (TopicNotFoundException ex)
        {
            _logger.LogError(ex, $"Topic with ID: {id} was not found for deletion");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting topic with ID: {id}");
            return StatusCode(500, "An error occurred while deleting the topic");
        }
    }
} 