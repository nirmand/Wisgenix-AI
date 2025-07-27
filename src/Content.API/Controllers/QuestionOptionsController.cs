using MediatR;
using Microsoft.AspNetCore.Mvc;
using Content.Application.Commands;
using Content.Application.DTOs;
using Content.Application.Queries;
using Content.Application.Validators;

namespace Content.API.Controllers;

/// <summary>
/// API Controller for QuestionOption management
/// </summary>
[ApiController]
[Route("api/content/question-options")]
public class QuestionOptionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAddQuestionOptionRequestValidator _addQuestionOptionValidator;
    private readonly IUpdateQuestionOptionRequestValidator _updateQuestionOptionValidator;

    public QuestionOptionsController(
        IMediator mediator,
        IAddQuestionOptionRequestValidator addQuestionOptionValidator,
        IUpdateQuestionOptionRequestValidator updateQuestionOptionValidator)
    {
        _mediator = mediator;
        _addQuestionOptionValidator = addQuestionOptionValidator;
        _updateQuestionOptionValidator = updateQuestionOptionValidator;
    }

    /// <summary>
    /// Get all question options
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<GetQuestionOptionsResponse>> GetQuestionOptions()
    {
        var query = new GetAllQuestionOptionsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get question option by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetQuestionOptionResponse>> GetQuestionOption(int id)
    {
        var query = new GetQuestionOptionByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (result == null)
        {
            return NotFound($"Question option with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Get question options by question ID
    /// </summary>
    [HttpGet("by-question/{questionId}")]
    public async Task<ActionResult<GetQuestionOptionsResponse>> GetQuestionOptionsByQuestion(int questionId)
    {
        var query = new GetQuestionOptionsByQuestionIdQuery(questionId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get correct options by question ID
    /// </summary>
    [HttpGet("by-question/{questionId}/correct")]
    public async Task<ActionResult<GetQuestionOptionsResponse>> GetCorrectOptionsByQuestion(int questionId)
    {
        var query = new GetCorrectOptionsByQuestionIdQuery(questionId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new question option
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<GetQuestionOptionResponse>> CreateQuestionOption([FromBody] AddQuestionOptionRequest request)
    {
        // Validate request
        var validationResult = await _addQuestionOptionValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var command = new CreateQuestionOptionCommand(request.OptionText, request.QuestionId, request.IsCorrect);
        var result = await _mediator.Send(command);
        
        return CreatedAtAction(nameof(GetQuestionOption), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing question option
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<GetQuestionOptionResponse>> UpdateQuestionOption(int id, [FromBody] UpdateQuestionOptionRequest request)
    {
        // Validate request
        var validationResult = await _updateQuestionOptionValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var command = new UpdateQuestionOptionCommand(id, request.OptionText, request.QuestionId, request.IsCorrect);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        catch (Wisgenix.SharedKernel.Domain.Exceptions.EntityNotFoundException)
        {
            return NotFound($"Question option with ID {id} not found");
        }
    }

    /// <summary>
    /// Delete a question option
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteQuestionOption(int id)
    {
        try
        {
            var command = new DeleteQuestionOptionCommand(id);
            await _mediator.Send(command);

            return NoContent();
        }
        catch (Wisgenix.SharedKernel.Domain.Exceptions.EntityNotFoundException)
        {
            return NotFound($"Question option with ID {id} not found");
        }
    }
}
