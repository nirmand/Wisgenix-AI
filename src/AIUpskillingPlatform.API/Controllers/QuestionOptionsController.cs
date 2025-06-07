using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Core.Logger;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.DTO;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AutoMapper;

namespace AIUpskillingPlatform.API.Controllers;

[ApiController]
[Route("api/content")]
public class QuestionOptionsController : ControllerBase
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
        var logContext = LogContext.Create("GetQuestionOptions");
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
        var logContext = LogContext.Create("GetQuestionOption");
        try
        {
            var option = await _questionOptionRepository.GetByIdAsync(logContext, id);
            var optionDto = _mapper.Map<QuestionOptionDto>(option);
            return Ok(optionDto);
        }
        catch (QuestionOptionNotFoundException ex)
        {
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
        var logContext = LogContext.Create("GetOptionsByQuestion");
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
    public async Task<ActionResult<QuestionOptionDto>> CreateQuestionOption(CreateQuestionOptionDto createOptionDto)
    {
        var logContext = LogContext.Create("CreateQuestionOption");
        try
        {
            if (!await _questionOptionRepository.QuestionExistsAsync(logContext, createOptionDto.QuestionID))
            {
                return BadRequest($"Question with ID {createOptionDto.QuestionID} does not exist");
            }

            var option = _mapper.Map<QuestionOption>(createOptionDto);
            var createdOption = await _questionOptionRepository.CreateAsync(logContext, option);
            var optionDto = _mapper.Map<QuestionOptionDto>(createdOption);

            return CreatedAtAction(nameof(GetQuestionOption), new { id = createdOption.ID }, optionDto);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<QuestionOption>(logContext, ex, "Error occurred while creating question option");
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while creating the question option");
        }
    }

    [HttpPut("update-option/{id}")]
    public async Task<IActionResult> UpdateQuestionOption(int id, UpdateQuestionOptionDto updateOptionDto)
    {
        var logContext = LogContext.Create("UpdateQuestionOption");
        try
        {
            if (!await _questionOptionRepository.QuestionExistsAsync(logContext, updateOptionDto.QuestionID))
            {
                return BadRequest($"Question with ID {updateOptionDto.QuestionID} does not exist");
            }

            var option = _mapper.Map<QuestionOption>(updateOptionDto);
            option.ID = id;
            
            await _questionOptionRepository.UpdateAsync(logContext, option);
            return NoContent();
        }
        catch (QuestionOptionNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<QuestionOption>(logContext, ex, $"Error occurred while updating question option {id}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while updating the question option");
        }
    }

    [HttpDelete("delete-option/{id}")]
    public async Task<IActionResult> DeleteQuestionOption(int id)
    {
        var logContext = LogContext.Create("DeleteQuestionOption");
        try
        {
            await _questionOptionRepository.DeleteAsync(logContext, id);
            return NoContent();
        }
        catch (QuestionOptionNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<QuestionOption>(logContext, ex, $"Error occurred while deleting question option {id}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while deleting the question option");
        }
    }
}