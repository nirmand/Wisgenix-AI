using System.Net;
using AutoMapper;
using Wisgenix.Common.Exceptions;
using Wisgenix.Core.Logger;
using Wisgenix.Data.Entities;
using Wisgenix.DTO;
using Wisgenix.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Wisgenix.API.Controllers;

[ApiController]
[Route("api/content")]
public class TopicsController : ControllerBase
{
    private readonly ITopicRepository _topicRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly ILoggingService _logger;
    private readonly IMapper _mapper;

    public TopicsController(ITopicRepository topicRepository, ISubjectRepository subjectRepository, ILoggingService logger, IMapper mapper)
    {
        _topicRepository = topicRepository;
        _subjectRepository = subjectRepository;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet("topics")]
    public async Task<ActionResult<IEnumerable<TopicDto>>> GetTopics()
    {
        LogContext logContext = LogContext.Create("GetTopics");
        try
        {
            var topics = await _topicRepository.GetAllAsync(logContext);
            var topicDtos = _mapper.Map<IEnumerable<TopicDto>>(topics);
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
            var topicDto = _mapper.Map<TopicDto>(topic);
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
        var userName = User?.Identity?.Name ?? "system";
        LogContext logContext = LogContext.Create("CreateTopic", userName);
        try
        {
            if (!await _subjectRepository.SubjectExistsAsync(logContext, createTopicDto.SubjectID))
            {
                _logger.LogOperationWarning<Topic>(logContext, $"Subject with ID {createTopicDto.SubjectID} does not exist");
                return BadRequest($"Subject with ID {createTopicDto.SubjectID} does not exist");
            }

            var topic = _mapper.Map<Topic>(createTopicDto);
            var createdTopic = await _topicRepository.CreateAsync(logContext, topic);
            var topicDto = _mapper.Map<TopicDto>(createdTopic);

            return CreatedAtAction(nameof(GetTopic), new { id = createdTopic.ID }, topicDto);
        }
        catch (DuplicateTopicException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, "Error occurred while creating topic");
            return StatusCode(500, "An error occurred while creating the topic");
        }
    }

    [HttpPut("update-topic/{id}")]
    public async Task<IActionResult> UpdateTopic(int id, UpdateTopicDto updateTopicDto)
    {
        var userName = User?.Identity?.Name ?? "system";
        LogContext logContext = LogContext.Create("UpdateTopic", userName);
        try
        {
            if (!await _subjectRepository.SubjectExistsAsync(logContext, updateTopicDto.SubjectID))
            {
                return BadRequest($"Subject with ID {updateTopicDto.SubjectID} does not exist");
            }

            var existingTopic = await _topicRepository.GetByIdAsync(logContext, id);
            if (existingTopic == null)
            {
                return NotFound($"Topic with ID {id} was not found");
            }

            _mapper.Map(updateTopicDto, existingTopic);
            await _topicRepository.UpdateAsync(logContext, existingTopic);
            return NoContent();
        }
        catch (DuplicateTopicException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (TopicNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, $"Error occurred while updating topic with ID: {id}");
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
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext,ex, $"Error occurred while deleting topic with ID: {id}");
            return StatusCode(500, "An error occurred while deleting the topic");
        }
    }

    [HttpGet("topics-by-subject/{subjectId}")]
    public async Task<ActionResult<IEnumerable<TopicDto>>> GetTopicsBySubject(int subjectId)
    {
        LogContext logContext = LogContext.Create("GetTopicsBySubject");
        try
        {
            var topics = await _topicRepository.GetBySubjectIdAsync(logContext, subjectId);
            var topicDtos = _mapper.Map<IEnumerable<TopicDto>>(topics);
            return Ok(topicDtos);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, $"Error occurred while getting topics for subject {subjectId}");
            return StatusCode(500, "An error occurred while retrieving topics by subject");
        }
    }
}