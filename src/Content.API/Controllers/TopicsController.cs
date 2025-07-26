using MediatR;
using Microsoft.AspNetCore.Mvc;
using Content.Application.Commands;
using Content.Application.DTOs;
using Content.Application.Queries;
using Content.Application.Validators;

namespace Content.API.Controllers;

/// <summary>
/// API Controller for Topic management
/// </summary>
[ApiController]
[Route("api/content/topics")]
public class TopicsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAddTopicRequestValidator _addTopicValidator;
    private readonly IUpdateTopicRequestValidator _updateTopicValidator;

    public TopicsController(
        IMediator mediator,
        IAddTopicRequestValidator addTopicValidator,
        IUpdateTopicRequestValidator updateTopicValidator)
    {
        _mediator = mediator;
        _addTopicValidator = addTopicValidator;
        _updateTopicValidator = updateTopicValidator;
    }

    /// <summary>
    /// Get all topics
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<GetTopicsResponse>> GetTopics([FromQuery] bool includeQuestions = false)
    {
        var query = new GetAllTopicsQuery(includeQuestions);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get topic by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetTopicResponse>> GetTopic(int id, [FromQuery] bool includeQuestions = false)
    {
        var query = new GetTopicByIdQuery(id, includeQuestions);
        var result = await _mediator.Send(query);
        
        if (result == null)
        {
            return NotFound($"Topic with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Get topics by subject ID
    /// </summary>
    [HttpGet("by-subject/{subjectId}")]
    public async Task<ActionResult<GetTopicsResponse>> GetTopicsBySubject(int subjectId, [FromQuery] bool includeQuestions = false)
    {
        var query = new GetTopicsBySubjectIdQuery(subjectId, includeQuestions);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new topic
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<GetTopicResponse>> CreateTopic([FromBody] AddTopicRequest request)
    {
        // Validate request
        var validationResult = await _addTopicValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var command = new CreateTopicCommand(request.TopicName, request.SubjectId);
        var result = await _mediator.Send(command);
        
        return CreatedAtAction(nameof(GetTopic), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing topic
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<GetTopicResponse>> UpdateTopic(int id, [FromBody] UpdateTopicRequest request)
    {
        // Validate request
        var validationResult = await _updateTopicValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var command = new UpdateTopicCommand(id, request.TopicName, request.SubjectId);
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }

    /// <summary>
    /// Delete a topic
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTopic(int id)
    {
        var command = new DeleteTopicCommand(id);
        await _mediator.Send(command);
        
        return NoContent();
    }
}
