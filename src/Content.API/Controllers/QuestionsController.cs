using MediatR;
using Microsoft.AspNetCore.Mvc;
using Content.Application.Commands;
using Content.Application.DTOs;
using Content.Application.Queries;
using Content.Application.Validators;

namespace Content.API.Controllers;

/// <summary>
/// API Controller for Question management
/// </summary>
[ApiController]
[Route("api/content/questions")]
public class QuestionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAddQuestionRequestValidator _addQuestionValidator;
    private readonly IUpdateQuestionRequestValidator _updateQuestionValidator;

    public QuestionsController(
        IMediator mediator,
        IAddQuestionRequestValidator addQuestionValidator,
        IUpdateQuestionRequestValidator updateQuestionValidator)
    {
        _mediator = mediator;
        _addQuestionValidator = addQuestionValidator;
        _updateQuestionValidator = updateQuestionValidator;
    }

    /// <summary>
    /// Get all questions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<GetQuestionsResponse>> GetQuestions([FromQuery] bool includeOptions = false)
    {
        var query = new GetAllQuestionsQuery(includeOptions);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get question by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetQuestionResponse>> GetQuestion(int id, [FromQuery] bool includeOptions = false)
    {
        var query = new GetQuestionByIdQuery(id, includeOptions);
        var result = await _mediator.Send(query);
        
        if (result == null)
        {
            return NotFound($"Question with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Get questions by topic ID
    /// </summary>
    [HttpGet("by-topic/{topicId}")]
    public async Task<ActionResult<GetQuestionsResponse>> GetQuestionsByTopic(int topicId, [FromQuery] bool includeOptions = false)
    {
        var query = new GetQuestionsByTopicIdQuery(topicId, includeOptions);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get questions by difficulty level
    /// </summary>
    [HttpGet("by-difficulty/{difficultyLevel}")]
    public async Task<ActionResult<GetQuestionsResponse>> GetQuestionsByDifficulty(int difficultyLevel, [FromQuery] bool includeOptions = false)
    {
        var query = new GetQuestionsByDifficultyQuery(difficultyLevel, includeOptions);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new question
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<GetQuestionResponse>> CreateQuestion([FromBody] AddQuestionRequest request)
    {
        // Validate request
        var validationResult = await _addQuestionValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var command = new CreateQuestionCommand(
            request.QuestionText, 
            request.TopicId, 
            request.DifficultyLevel, 
            request.MaxScore, 
            request.GeneratedBy, 
            request.QuestionSourceReference);
        
        var result = await _mediator.Send(command);
        
        return CreatedAtAction(nameof(GetQuestion), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing question
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<GetQuestionResponse>> UpdateQuestion(int id, [FromBody] UpdateQuestionRequest request)
    {
        // Validate request
        var validationResult = await _updateQuestionValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var command = new UpdateQuestionCommand(
            id,
            request.QuestionText, 
            request.TopicId, 
            request.DifficultyLevel, 
            request.MaxScore, 
            request.GeneratedBy, 
            request.QuestionSourceReference);
        
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }

    /// <summary>
    /// Delete a question
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteQuestion(int id)
    {
        var command = new DeleteQuestionCommand(id);
        await _mediator.Send(command);
        
        return NoContent();
    }
}
