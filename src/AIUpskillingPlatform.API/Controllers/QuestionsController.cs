using AIUpskillingPlatform.API.DTOs;
using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Core.Logger;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIUpskillingPlatform.API.Controllers;

[ApiController]
[Route("api/content")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ILoggingService _logger;

    public QuestionsController(IQuestionRepository questionRepository, ILoggingService logger)
    {
        _questionRepository = questionRepository;
        _logger = logger;
    }

    [HttpGet("questions")]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestions()
    {
        try
        {
            _logger.LogInformation("Getting all questions");
            var questions = await _questionRepository.GetAllAsync();
            var questionDtos = questions.Select(q => new QuestionDto
            {
                ID = q.ID,
                QuestionText = q.QuestionText,
                TopicID = q.TopicID,
                DifficultyLevel = q.DifficultyLevel,
                MaxScore = q.MaxScore,
                GeneratedBy = q.GeneratedBy,
                TopicName = q.Topic?.TopicName ?? string.Empty
            });
            _logger.LogInformation($"Successfully retrieved {questions.Count()} questions");
            return Ok(questionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all questions");
            return StatusCode(500, "An error occurred while retrieving questions");
        }
    }

    [HttpGet("question/{id}")]
    public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
    {
        try
        {
            _logger.LogInformation($"Getting question with ID: {id}");
            var question = await _questionRepository.GetByIdAsync(id);
            var questionDto = new QuestionDto
            {
                ID = question.ID,
                QuestionText = question.QuestionText,
                TopicID = question.TopicID,
                DifficultyLevel = question.DifficultyLevel,
                MaxScore = question.MaxScore,
                GeneratedBy = question.GeneratedBy,
                TopicName = question.Topic?.TopicName ?? string.Empty
            };
            _logger.LogInformation($"Successfully retrieved question with ID: {id}");
            return Ok(questionDto);
        }
        catch (QuestionNotFoundException ex)
        {
            _logger.LogError(ex, $"Question with ID: {id} was not found");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting question with ID: {id}");
            return StatusCode(500, "An error occurred while retrieving the question");
        }
    }

    [HttpPost("create-question")]
    public async Task<ActionResult<QuestionDto>> CreateQuestion(CreateQuestionDto createQuestionDto)
    {
        try
        {
            _logger.LogInformation($"Creating new question for topic ID: {createQuestionDto.TopicID}");

            // Validate if topic exists
            if (!await _questionRepository.TopicExistsAsync(createQuestionDto.TopicID))
            {
                _logger.LogError(new TopicNotFoundException(createQuestionDto.TopicID), $"Topic does not exist");
                return BadRequest($"Topic with ID {createQuestionDto.TopicID} does not exist");
            }

            var question = new Question
            {
                QuestionText = createQuestionDto.QuestionText,
                TopicID = createQuestionDto.TopicID,
                DifficultyLevel = createQuestionDto.DifficultyLevel,
                MaxScore = createQuestionDto.MaxScore,
                GeneratedBy = createQuestionDto.GeneratedBy,
                QuestionSourceReference = createQuestionDto.QuestionSourceReference
            };

            var createdQuestion = await _questionRepository.CreateAsync(question);
            var questionDto = new QuestionDto
            {
                ID = createdQuestion.ID,
                QuestionText = createdQuestion.QuestionText,
                TopicID = createdQuestion.TopicID,
                DifficultyLevel = createdQuestion.DifficultyLevel,
                MaxScore = createdQuestion.MaxScore,
                GeneratedBy = createdQuestion.GeneratedBy
            };

            _logger.LogInformation($"Successfully created question with ID: {createdQuestion.ID}");
            return CreatedAtAction(nameof(GetQuestion), new { id = createdQuestion.ID }, questionDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while creating question for topic ID: {createQuestionDto.TopicID}");
            return StatusCode(500, "An error occurred while creating the question");
        }
    }

    [HttpPut("update-question/{id}")]
    public async Task<IActionResult> UpdateQuestion(int id, UpdateQuestionDto updateQuestionDto)
    {
        try
        {
            _logger.LogInformation($"Updating question with ID: {id}");

            // Validate if topic exists
            if (!await _questionRepository.TopicExistsAsync(updateQuestionDto.TopicID))
            {
                _logger.LogError(new TopicNotFoundException(updateQuestionDto.TopicID), $"Topic with ID: {updateQuestionDto.TopicID} does not exist");
                return BadRequest($"Topic with ID {updateQuestionDto.TopicID} does not exist");
            }

            var question = await _questionRepository.GetByIdAsync(id);
            if (question == null)
            {
                _logger.LogError(new QuestionNotFoundException(id), $"Question with ID: {id} was not found for update");
                return NotFound($"Question with ID {id} was not found");
            }

            question.QuestionText = updateQuestionDto.QuestionText;
            question.TopicID = updateQuestionDto.TopicID;
            question.DifficultyLevel = updateQuestionDto.DifficultyLevel;
            question.MaxScore = updateQuestionDto.MaxScore;
            question.GeneratedBy = updateQuestionDto.GeneratedBy;
            question.QuestionSourceReference = updateQuestionDto.QuestionSourceReference;

            await _questionRepository.UpdateAsync(question);
            _logger.LogInformation($"Successfully updated question with ID: {id}");
            return NoContent();
        }
        catch (QuestionNotFoundException ex)
        {
            _logger.LogError(ex, $"Question with ID: {id} was not found for update");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating question with ID: {id}");
            return StatusCode(500, "An error occurred while updating the question");
        }
    }

    [HttpDelete("delete-question/{id}")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        try
        {
            _logger.LogInformation($"Deleting question with ID: {id}");
            await _questionRepository.DeleteAsync(id);
            _logger.LogInformation($"Successfully deleted question with ID: {id}");
            return NoContent();
        }
        catch (QuestionNotFoundException ex)
        {
            _logger.LogError(ex, $"Question with ID: {id} was not found for deletion");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting question with ID: {id}");
            return StatusCode(500, "An error occurred while deleting the question");
        }
    }
}