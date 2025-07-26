using MediatR;
using Microsoft.AspNetCore.Mvc;
using Content.Application.Commands;
using Content.Application.DTOs;
using Content.Application.Queries;
using Content.Application.Validators;

namespace Content.API.Controllers;

/// <summary>
/// API Controller for Subject management
/// </summary>
[ApiController]
[Route("api/content/subjects")]
public class SubjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAddSubjectRequestValidator _addSubjectValidator;
    private readonly IUpdateSubjectRequestValidator _updateSubjectValidator;

    public SubjectsController(
        IMediator mediator,
        IAddSubjectRequestValidator addSubjectValidator,
        IUpdateSubjectRequestValidator updateSubjectValidator)
    {
        _mediator = mediator;
        _addSubjectValidator = addSubjectValidator;
        _updateSubjectValidator = updateSubjectValidator;
    }

    /// <summary>
    /// Get all subjects
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<GetSubjectsResponse>> GetSubjects([FromQuery] bool includeTopics = false)
    {
        var query = new GetAllSubjectsQuery(includeTopics);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get subject by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetSubjectResponse>> GetSubject(int id, [FromQuery] bool includeTopics = false)
    {
        var query = new GetSubjectByIdQuery(id, includeTopics);
        var result = await _mediator.Send(query);
        
        if (result == null)
        {
            return NotFound($"Subject with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new subject
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<GetSubjectResponse>> CreateSubject([FromBody] AddSubjectRequest request)
    {
        // Validate request
        var validationResult = await _addSubjectValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var command = new CreateSubjectCommand(request.SubjectName);
        var result = await _mediator.Send(command);
        
        return CreatedAtAction(nameof(GetSubject), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing subject
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<GetSubjectResponse>> UpdateSubject(int id, [FromBody] UpdateSubjectRequest request)
    {
        // Validate request
        var validationResult = await _updateSubjectValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var command = new UpdateSubjectCommand(id, request.SubjectName);
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }

    /// <summary>
    /// Delete a subject
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSubject(int id)
    {
        var command = new DeleteSubjectCommand(id);
        await _mediator.Send(command);
        
        return NoContent();
    }
}
