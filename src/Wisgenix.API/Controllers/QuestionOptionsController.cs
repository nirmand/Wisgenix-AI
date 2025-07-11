using Wisgenix.Common.Exceptions;
using Wisgenix.Core.Logger;
using Wisgenix.Data.Entities;
using Wisgenix.DTO;
using Wisgenix.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AutoMapper;

namespace Wisgenix.API.Controllers;

[ApiController]
[Route("api/content")]
public class QuestionOptionsController : BaseApiController
{
    private readonly IQuestionOptionRepository _questionOptionRepository;
    private readonly ILoggingService _logger;
    private readonly IMapper _mapper;

    public QuestionOptionsController(IQuestionOptionRepository questionOptionRepository, ILoggingService logger, IMapper mapper)
    {
        _questionOptionRepository = questionOptionRepository;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet("options")]
    public async Task<ActionResult<IEnumerable<QuestionOptionDto>>> GetQuestionOptions()
    {
        var logContext = CreateLogContext("GetQuestionOptions");
        try
        {
            var options = await _questionOptionRepository.GetAllAsync(logContext);
            var optionDtos = _mapper.Map<IEnumerable<QuestionOptionDto>>(options);
            return Ok(optionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<QuestionOption>(logContext, ex, "Error occurred while getting all question options");
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while retrieving question options");
        }
    }

    [HttpGet("option/{id}")]
    public async Task<ActionResult<QuestionOptionDto>> GetQuestionOption(int id)
    {
        var logContext = CreateLogContext("GetQuestionOption");
        try
        {
            var option = await _questionOptionRepository.GetByIdAsync(logContext, id);
            var optionDto = _mapper.Map<QuestionOptionDto>(option);
            return Ok(optionDto);
        }
        catch (QuestionOptionNotFoundException ex)
        {
            _logger.LogOperationWarning<QuestionOption>(logContext, ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<QuestionOption>(logContext, ex, $"Error occurred while getting question option {id}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while retrieving the question option");
        }
    }

    [HttpGet("options-by-question/{questionId}")]
    public async Task<ActionResult<IEnumerable<QuestionOptionDto>>> GetOptionsByQuestion(int questionId)
    {
        var logContext = CreateLogContext("GetOptionsByQuestion");
        try
        {
            var options = await _questionOptionRepository.GetByQuestionIdAsync(logContext, questionId);
            var optionDtos = _mapper.Map<IEnumerable<QuestionOptionDto>>(options);
            return Ok(optionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<QuestionOption>(logContext, ex, $"Error occurred while getting options for question {questionId}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while retrieving question options");
        }
    }

    [HttpPost("create-option")]
    public async Task<ActionResult<QuestionOptionDto>> CreateQuestionOption([FromBody] CreateQuestionOptionDto createQuestionOptionDto)
    {
        var userName = User?.Identity?.Name ?? "system";
        var logContext = CreateLogContext("CreateQuestionOption", userName);
        try
        {
            if (!await _questionOptionRepository.QuestionExistsAsync(logContext, createQuestionOptionDto.QuestionID))
            {
                _logger.LogOperationWarning<QuestionOption>(logContext, $"Question with ID {createQuestionOptionDto.QuestionID} does not exist");
                return BadRequest($"Question with ID {createQuestionOptionDto.QuestionID} does not exist");
            }
            var option = _mapper.Map<QuestionOption>(createQuestionOptionDto);
            var createdOption = await _questionOptionRepository.CreateAsync(logContext, option);
            var optionDto = _mapper.Map<QuestionOptionDto>(createdOption);
            return CreatedAtAction(nameof(GetQuestionOption), new { id = createdOption.ID }, optionDto);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<QuestionOption>(logContext, ex, "Error occurred while creating question option");
            return StatusCode(500, "An error occurred while creating the question option");
        }
    }

    [HttpPut("update-option/{id}")]
    public async Task<IActionResult> UpdateQuestionOption(int id, UpdateQuestionOptionDto updateQuestionOptionDto)
    {
        var userName = User?.Identity?.Name ?? "system";
        var logContext = CreateLogContext("UpdateQuestionOption", userName);
        try
        {
            var existingOption = await _questionOptionRepository.GetByIdAsync(logContext, id);
            if (existingOption == null)
            {
                _logger.LogOperationWarning<QuestionOption>(logContext, $"Question option with ID {id} was not found");
                return NotFound($"Question option with ID {id} was not found");
            }
            _mapper.Map(updateQuestionOptionDto, existingOption);
            await _questionOptionRepository.UpdateAsync(logContext, existingOption);
            return NoContent();
        }
        catch (QuestionOptionNotFoundException ex)
        {
            _logger.LogOperationWarning<QuestionOption>(logContext, ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<QuestionOption>(logContext, ex, $"Error occurred while updating question option with ID: {id}");
            return StatusCode(500, "An error occurred while updating the question option");
        }
    }

    [HttpDelete("delete-option/{id}")]
    public async Task<IActionResult> DeleteQuestionOption(int id)
    {
        var logContext = CreateLogContext("DeleteQuestionOption");
        try
        {
            _logger.LogInformation($"Deleting question option with ID: {id}");
            await _questionOptionRepository.DeleteAsync(logContext, id);
            _logger.LogInformation($"Successfully deleted question option with ID: {id}");
            return NoContent();
        }
        catch (QuestionOptionNotFoundException ex)
        {
            _logger.LogOperationWarning<QuestionOption>(logContext, ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<QuestionOption>(logContext, ex, $"Error occurred while deleting question option {id}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while deleting the question option");
        }
    }
}