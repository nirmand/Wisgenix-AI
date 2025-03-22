using AIUpskillingPlatform.API.DTOs;
using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AIUpskillingPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TopicsController : ControllerBase
{
    private readonly ITopicRepository _topicRepository;
    private readonly ILogger<TopicsController> _logger;

    public TopicsController(ITopicRepository topicRepository, ILogger<TopicsController> logger)
    {
        _topicRepository = topicRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TopicDto>>> GetTopics()
    {
        try
        {
            _logger.LogInformation("Getting all topics");
            var topics = await _topicRepository.GetAllAsync();
            var topicDtos = topics.Select(t => new TopicDto { ID = t.ID, TopicName = t.TopicName });
            _logger.LogInformation("Successfully retrieved {Count} topics", topics.Count());
            return Ok(topicDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all topics");
            return StatusCode(500, "An error occurred while retrieving topics");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TopicDto>> GetTopic(int id)
    {
        try
        {
            _logger.LogInformation("Getting topic with ID: {Id}", id);
            var topic = await _topicRepository.GetByIdAsync(id);
            var topicDto = new TopicDto { ID = topic.ID, TopicName = topic.TopicName };
            _logger.LogInformation("Successfully retrieved topic with ID: {Id}", id);
            return Ok(topicDto);
        }
        catch (TopicNotFoundException ex)
        {
            _logger.LogWarning(ex, "Topic with ID: {Id} was not found", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting topic with ID: {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the topic");
        }
    }

    [HttpPost]
    public async Task<ActionResult<TopicDto>> CreateTopic(CreateTopicDto createTopicDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid topic name");
            }
            _logger.LogInformation("Creating new topic with name: {TopicName}", createTopicDto.TopicName);
            var topic = new Topic { TopicName = createTopicDto.TopicName };
            var createdTopic = await _topicRepository.CreateAsync(topic);
            var topicDto = new TopicDto { ID = createdTopic.ID, TopicName = createdTopic.TopicName };
            _logger.LogInformation("Successfully created topic with ID: {Id}", createdTopic.ID);
            return CreatedAtAction(nameof(GetTopic), new { id = createdTopic.ID }, topicDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating topic with name: {TopicName}", createTopicDto.TopicName);
            return StatusCode(500, "An error occurred while creating the topic");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTopic(int id, UpdateTopicDto updateTopicDto)
    {
        try
        {
            _logger.LogInformation("Updating topic with ID: {Id}", id);
            var topic = new Topic { ID = id, TopicName = updateTopicDto.TopicName };
            await _topicRepository.UpdateAsync(topic);
            _logger.LogInformation("Successfully updated topic with ID: {Id}", id);
            return NoContent();
        }
        catch (TopicNotFoundException ex)
        {
            _logger.LogWarning(ex, "Topic with ID: {Id} was not found for update", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating topic with ID: {Id}", id);
            return StatusCode(500, "An error occurred while updating the topic");
        }
    }

    [HttpDelete("{id}")]
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
            _logger.LogWarning(ex, "Topic with ID: {Id} was not found for deletion", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting topic with ID: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the topic");
        }
    }
} 