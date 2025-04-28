using System.Net;
using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Core.Logger;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.DTO;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        LogContext logContext = LogContext.Create("GetTopics");
        try
        {
            var topics = await _topicRepository.GetAllAsync(logContext);
            var topicDtos = topics.Select(t => new TopicDto { ID = t.ID, TopicName = t.TopicName, SubjectID = t.SubjectID });
            return Ok(topicDtos);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, "Unhandled error getting all topics");
            return StatusCode(500, "An error occurred while retrieving topics");
        }
    }

    [HttpGet("get-topic/{id}")]
    public async Task<ActionResult<TopicDto>> GetTopic(int id)
    {
        LogContext logContext = LogContext.Create("GetTopic");
        try
        {
            var topic = await _topicRepository.GetByIdAsync(logContext, id);
            var topicDto = new TopicDto { ID = topic.ID, TopicName = topic.TopicName, SubjectID = topic.SubjectID };
            return Ok(topicDto);
        }
        catch (TopicNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, $"Unhandled error getting topic {id}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while retrieving the topic");
        }
    }

    [HttpPost("create-topic")]
    public async Task<ActionResult<TopicDto>> CreateTopic([FromBody] CreateTopicDto createTopicDto)
    {
        LogContext logContext = LogContext.Create("CreateTopic");
        try
        {
            // Check if subject exists
            if (!await _topicRepository.SubjectExistsAsync(logContext, createTopicDto.SubjectID))
            {
                _logger.LogOperationWarning<Topic>(logContext, "Subject with ID {SubjectID} does not exist", createTopicDto.SubjectID);
                return BadRequest(ModelState);
            }

            var topic = new Topic
            {
                TopicName = createTopicDto.TopicName,
                SubjectID = createTopicDto.SubjectID
            };

            var createdTopic = await _topicRepository.CreateAsync(logContext, topic);

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
        LogContext logContext = LogContext.Create("UpdateTopic");
        try
        {
            // Check if subject exists
            if (!await _topicRepository.SubjectExistsAsync(logContext, updateTopicDto.SubjectID))
            {
                return BadRequest($"Subject with ID {updateTopicDto.SubjectID} does not exist");
            }

            var topic = await _topicRepository.GetByIdAsync(logContext, id);
            if (topic == null)
            {
                return NotFound($"Topic with ID {id} was not found");
            }

            topic.TopicName = updateTopicDto.TopicName;
            topic.SubjectID = updateTopicDto.SubjectID;

            await _topicRepository.UpdateAsync(logContext, topic);
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
        LogContext logContext = LogContext.Create("DeleteTopic");
        try
        {
            _logger.LogInformation("Deleting topic with ID: {Id}", id);
            await _topicRepository.DeleteAsync(logContext, id);
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