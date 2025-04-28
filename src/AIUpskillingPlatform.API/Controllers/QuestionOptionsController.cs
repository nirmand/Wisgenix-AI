using AIUpskillingPlatform.API.DTOs;
using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Core.Logger;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIUpskillingPlatform.API.Controllers;

[ApiController]
[Route("api/content")]
public class QuestionOptionsController : ControllerBase
{
    private readonly IQuestionOptionRepository _questionOptionRepository;
    private readonly ILoggingService _logger;

    public QuestionOptionsController(IQuestionOptionRepository questionOptionRepository, ILoggingService logger)
    {
        _questionOptionRepository = questionOptionRepository;
        _logger = logger;
    }

    [HttpGet("options")]
    public async Task<ActionResult<IEnumerable<QuestionOptionDto>>> GetQuestionOptions()
    {
        try
        {
            _logger.LogInformation("Getting all question options");
            var options = await _questionOptionRepository.GetAllAsync();
            var optionDtos = options.Select(o => new QuestionOptionDto
            {
                ID = o.ID,
                OptionText = o.OptionText,
                IsCorrect = o.IsCorrect,
                QuestionID = o.QuestionID
            });
            _logger.LogInformation($"Successfully retrieved {options.Count()} question options");
            return Ok(optionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all question options");
            return StatusCode(500, "An error occurred while retrieving question options");
        }
    }

    [HttpGet("option/{id}")]
    public async Task<ActionResult<QuestionOptionDto>> GetQuestionOption(int id)
    {
        try
        {
            _logger.LogInformation($"Getting question option with ID: {id}");
            var option = await _questionOptionRepository.GetByIdAsync(id);
            var optionDto = new QuestionOptionDto
            {
                ID = option.ID,
                OptionText = option.OptionText,
                IsCorrect = option.IsCorrect,
                QuestionID = option.QuestionID
            };
            _logger.LogInformation($"Successfully retrieved question option with ID: {id}");
            return Ok(optionDto);
        }
        catch (QuestionOptionNotFoundException ex)
        {
            _logger.LogError(ex, $"Question option with ID: {id} was not found");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting question option with ID: {id}");
            return StatusCode(500, "An error occurred while retrieving the question option");
        }
    }

    [HttpGet("options-for-question/{questionId}")]
    public async Task<ActionResult<IEnumerable<QuestionOptionDto>>> GetOptionsByQuestion(int questionId)
    {
        try
        {
            _logger.LogInformation($"Getting options for question ID: {questionId}");
            var options = await _questionOptionRepository.GetByQuestionIdAsync(questionId);
            var optionDtos = options.Select(o => new QuestionOptionDto
            {
                ID = o.ID,
                OptionText = o.OptionText,
                IsCorrect = o.IsCorrect,
                QuestionID = o.QuestionID
            });
            _logger.LogInformation($"Successfully retrieved {options.Count()} options for question ID: {questionId}");
            return Ok(optionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting options for question ID: {questionId}");
            return StatusCode(500, "An error occurred while retrieving question options");
        }
    }

    [HttpPost("create-option")]
    public async Task<ActionResult<QuestionOptionDto>> CreateQuestionOption(CreateQuestionOptionDto createOptionDto)
    {
        try
        {
            _logger.LogInformation($"Creating new option for question ID: {createOptionDto.QuestionID}");

            // Validate if question exists
            if (!await _questionOptionRepository.QuestionExistsAsync(createOptionDto.QuestionID))
            {
                _logger.LogWarning($"Question with ID: {createOptionDto.QuestionID} does not exist");
                return BadRequest($"Question with ID {createOptionDto.QuestionID} does not exist");
            }

            var option = new QuestionOption
            {
                OptionText = createOptionDto.OptionText,
                IsCorrect = createOptionDto.IsCorrect,
                QuestionID = createOptionDto.QuestionID
            };

            var createdOption = await _questionOptionRepository.CreateAsync(option);
            var optionDto = new QuestionOptionDto
            {
                ID = createdOption.ID,
                OptionText = createdOption.OptionText,
                IsCorrect = createdOption.IsCorrect,
                QuestionID = createdOption.QuestionID
            };

            _logger.LogInformation($"Successfully created option with ID: {createdOption.ID}");
            return CreatedAtAction(nameof(GetQuestionOption), new { id = createdOption.ID }, optionDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while creating option for question ID: {createOptionDto.QuestionID}");
            return StatusCode(500, "An error occurred while creating the question option");
        }
    }

    [HttpPut("update-option/{id}")]
    public async Task<IActionResult> UpdateQuestionOption(int id, UpdateQuestionOptionDto updateOptionDto)
    {
        try
        {
            _logger.LogInformation($"Updating question option with ID: {id}");

            // Validate if question exists
            if (!await _questionOptionRepository.QuestionExistsAsync(updateOptionDto.QuestionID))
            {
                _logger.LogWarning($"Question with ID: {updateOptionDto.QuestionID} does not exist");
                return BadRequest($"Question with ID {updateOptionDto.QuestionID} does not exist");
            }

            var option = new QuestionOption
            {
                ID = id,
                OptionText = updateOptionDto.OptionText,
                IsCorrect = updateOptionDto.IsCorrect,
                QuestionID = updateOptionDto.QuestionID
            };

            await _questionOptionRepository.UpdateAsync(option);
            _logger.LogInformation($"Successfully updated question option with ID: {id}");
            return NoContent();
        }
        catch (QuestionOptionNotFoundException ex)
        {
            _logger.LogError(ex, $"Question option with ID: {id} was not found for update");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating question option with ID: {id}");
            return StatusCode(500, "An error occurred while updating the question option");
        }
    }

    [HttpDelete("delete-option/{id}")]
    public async Task<IActionResult> DeleteQuestionOption(int id)
    {
        try
        {
            _logger.LogInformation($"Deleting question option with ID: {id}");
            await _questionOptionRepository.DeleteAsync(id);
            _logger.LogInformation($"Successfully deleted question option with ID: {id}");
            return NoContent();
        }
        catch (QuestionOptionNotFoundException ex)
        {
            _logger.LogError(ex, $"Question option with ID: {id} was not found for deletion");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting question option with ID: {id}");
            return StatusCode(500, "An error occurred while deleting the question option");
        }
    }
}