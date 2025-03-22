using AIUpskillingPlatform.API.DTOs;
using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AIUpskillingPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ILogger<QuestionsController> _logger;

    public QuestionsController(IQuestionRepository questionRepository, ILogger<QuestionsController> logger)
    {
        _questionRepository = questionRepository;
        _logger = logger;
    }

    [HttpGet]
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
            _logger.LogInformation("Successfully retrieved {Count} questions", questions.Count());
            return Ok(questionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all questions");
            return StatusCode(500, "An error occurred while retrieving questions");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
    {
        try
        {
            _logger.LogInformation("Getting question with ID: {Id}", id);
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
            _logger.LogInformation("Successfully retrieved question with ID: {Id}", id);
            return Ok(questionDto);
        }
        catch (QuestionNotFoundException ex)
        {
            _logger.LogWarning(ex, "Question with ID: {Id} was not found", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting question with ID: {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the question");
        }
    }

    [HttpPost]
    public async Task<ActionResult<QuestionDto>> CreateQuestion(CreateQuestionDto createQuestionDto)
    {
        try
        {
            _logger.LogInformation("Creating new question for topic ID: {TopicId}", createQuestionDto.TopicID);
            
            // Validate if topic exists
            if (!await _questionRepository.TopicExistsAsync(createQuestionDto.TopicID))
            {
                _logger.LogWarning("Topic with ID: {TopicId} does not exist", createQuestionDto.TopicID);
                return BadRequest($"Topic with ID {createQuestionDto.TopicID} does not exist");
            }

            var question = new Question
            {
                QuestionText = createQuestionDto.QuestionText,
                TopicID = createQuestionDto.TopicID,
                DifficultyLevel = createQuestionDto.DifficultyLevel,
                MaxScore = createQuestionDto.MaxScore,
                GeneratedBy = createQuestionDto.GeneratedBy
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

            _logger.LogInformation("Successfully created question with ID: {Id}", createdQuestion.ID);
            return CreatedAtAction(nameof(GetQuestion), new { id = createdQuestion.ID }, questionDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating question for topic ID: {TopicId}", createQuestionDto.TopicID);
            return StatusCode(500, "An error occurred while creating the question");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuestion(int id, UpdateQuestionDto updateQuestionDto)
    {
        try
        {
            _logger.LogInformation("Updating question with ID: {Id}", id);

            // Validate if topic exists
            if (!await _questionRepository.TopicExistsAsync(updateQuestionDto.TopicID))
            {
                _logger.LogWarning("Topic with ID: {TopicId} does not exist", updateQuestionDto.TopicID);
                return BadRequest($"Topic with ID {updateQuestionDto.TopicID} does not exist");
            }

            var question = new Question
            {
                ID = id,
                QuestionText = updateQuestionDto.QuestionText,
                TopicID = updateQuestionDto.TopicID,
                DifficultyLevel = updateQuestionDto.DifficultyLevel,
                MaxScore = updateQuestionDto.MaxScore,
                GeneratedBy = updateQuestionDto.GeneratedBy
            };

            await _questionRepository.UpdateAsync(question);
            _logger.LogInformation("Successfully updated question with ID: {Id}", id);
            return NoContent();
        }
        catch (QuestionNotFoundException ex)
        {
            _logger.LogWarning(ex, "Question with ID: {Id} was not found for update", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating question with ID: {Id}", id);
            return StatusCode(500, "An error occurred while updating the question");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        try
        {
            _logger.LogInformation("Deleting question with ID: {Id}", id);
            await _questionRepository.DeleteAsync(id);
            _logger.LogInformation("Successfully deleted question with ID: {Id}", id);
            return NoContent();
        }
        catch (QuestionNotFoundException ex)
        {
            _logger.LogWarning(ex, "Question with ID: {Id} was not found for deletion", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting question with ID: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the question");
        }
    }
} 