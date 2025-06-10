using Wisgenix.Common.Exceptions;
using Wisgenix.Core.Logger;
using Wisgenix.Data.Entities;
using Wisgenix.DTO;
using Wisgenix.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Wisgenix.API.Controllers;

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
        var logContext = new LogContext();
        try
        {
            var questions = await _questionRepository.GetAllAsync(logContext);
            var questionDtos = questions.Select(q => new QuestionDto
            {
                ID = q.ID,
                QuestionText = q.QuestionText,
                TopicID = q.TopicID,
                DifficultyLevel = q.DifficultyLevel,
                MaxScore = q.MaxScore,
                GeneratedBy = q.GeneratedBy,
                TopicName = q.Topic?.TopicName ?? string.Empty,
                QuestionSourceReference = q.QuestionSourceReference
            });
            return Ok(questionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Subject>(logContext, ex, "Error occurred while getting all questions");
            return StatusCode(500, "An error occurred while retrieving questions");
        }
    }

    [HttpGet("question/{id}")]
    public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
    {
        var logContext = new LogContext();
        try
        {
            var question = await _questionRepository.GetByIdAsync(logContext, id);
            var questionDto = new QuestionDto
            {
                ID = question.ID,
                QuestionText = question.QuestionText,
                TopicID = question.TopicID,
                DifficultyLevel = question.DifficultyLevel,
                MaxScore = question.MaxScore,
                GeneratedBy = question.GeneratedBy,
                TopicName = question.Topic?.TopicName ?? string.Empty,
                QuestionSourceReference = question.QuestionSourceReference,
                CreatedDate = question.CreatedDate,
                CreatedBy = question.CreatedBy,
                ModifiedDate = question.ModifiedDate,
                ModifiedBy = question.ModifiedBy
            };
            return Ok(questionDto);
        }
        catch (QuestionNotFoundException ex)
        {
            _logger.LogOperationError<Subject>(logContext, ex, $"Question with ID: {id} was not found");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Subject>(logContext, ex, "Error occurred while getting question");
            return StatusCode(500, "An error occurred while retrieving the question");
        }
    }

    [HttpPost("create-question")]
    public async Task<ActionResult<QuestionDto>> CreateQuestion([FromBody] CreateQuestionDto createQuestionDto)
    {
        var userName = User?.Identity?.Name ?? "system";
        LogContext logContext = LogContext.Create("CreateQuestion", userName);
        try
        {
            if (!await _questionRepository.TopicExistsAsync(logContext, createQuestionDto.TopicID))
            {
                return BadRequest($"Topic with ID {createQuestionDto.TopicID} does not exist");
            }

            var question = new Question
            {
                QuestionText = createQuestionDto.QuestionText,
                TopicID = createQuestionDto.TopicID,
                DifficultyLevel = createQuestionDto.DifficultyLevel,
                MaxScore = createQuestionDto.MaxScore,
                GeneratedBy = createQuestionDto.GeneratedBy,
                QuestionSourceReference = createQuestionDto.QuestionSourceReference ?? string.Empty
            };

            var createdQuestion = await _questionRepository.CreateAsync(logContext, question);
            var questionDto = new QuestionDto
            {
                ID = createdQuestion.ID,
                QuestionText = createdQuestion.QuestionText,
                TopicID = createdQuestion.TopicID,
                DifficultyLevel = createdQuestion.DifficultyLevel,
                MaxScore = createdQuestion.MaxScore,
                GeneratedBy = createdQuestion.GeneratedBy,
                TopicName = createdQuestion.Topic?.TopicName ?? string.Empty,
                QuestionSourceReference = createdQuestion.QuestionSourceReference,
                CreatedDate = createdQuestion.CreatedDate,
                CreatedBy = createdQuestion.CreatedBy,
                ModifiedDate = createdQuestion.ModifiedDate,
                ModifiedBy = createdQuestion.ModifiedBy
            };

            return CreatedAtAction(nameof(GetQuestion), new { id = createdQuestion.ID }, questionDto);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Question>(logContext, ex, "Error occurred while creating question");
            return StatusCode(500, "An error occurred while creating the question");
        }
    }

    [HttpPut("update-question/{id}")]
    public async Task<IActionResult> UpdateQuestion(int id, UpdateQuestionDto updateQuestionDto)
    {
        var userName = User?.Identity?.Name ?? "system";
        LogContext logContext = LogContext.Create("UpdateQuestion", userName);
        try
        {
            if (!await _questionRepository.TopicExistsAsync(logContext, updateQuestionDto.TopicID))
            {
                return BadRequest($"Topic with ID {updateQuestionDto.TopicID} does not exist");
            }

            var question = new Question
            {
                ID = id,
                QuestionText = updateQuestionDto.QuestionText,
                TopicID = updateQuestionDto.TopicID,
                DifficultyLevel = updateQuestionDto.DifficultyLevel,
                MaxScore = updateQuestionDto.MaxScore,
                GeneratedBy = updateQuestionDto.GeneratedBy,
                QuestionSourceReference = updateQuestionDto.QuestionSourceReference ?? string.Empty
            };

            await _questionRepository.UpdateAsync(logContext, question);
            return NoContent();
        }
        catch (QuestionNotFoundException ex)
        {
            _logger.LogOperationError<Subject>(logContext, ex, $"Question with ID: {id} was not found");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Question>(logContext, ex, $"Error occurred while updating question with ID: {id}");
            return StatusCode(500, "An error occurred while updating the question");
        }
    }

    [HttpDelete("delete-question/{id}")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var logContext = new LogContext();
        try
        {
            await _questionRepository.DeleteAsync(logContext, id);
            return NoContent();
        }
        catch (QuestionNotFoundException ex)
        {
            _logger.LogOperationError<Subject>(logContext, ex, $"Question with ID: {id} was not found");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Subject>(logContext, ex, "Error occurred while deleting question");
            return StatusCode(500, "An error occurred while deleting the question");
        }
    }

    [HttpGet("questions-by-topic/{topicId}")]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestionsByTopic(int topicId)
    {
        var logContext = new LogContext();
        try
        {
            var questions = await _questionRepository.GetByTopicIdAsync(logContext, topicId);
            var questionDtos = questions.Select(q => new QuestionDto
            {
                ID = q.ID,
                QuestionText = q.QuestionText,
                TopicID = q.TopicID,
                DifficultyLevel = q.DifficultyLevel,
                MaxScore = q.MaxScore,
                GeneratedBy = q.GeneratedBy,
                TopicName = q.Topic?.TopicName ?? string.Empty,
                QuestionSourceReference = q.QuestionSourceReference
            });
            return Ok(questionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Subject>(logContext, ex, $"Error occurred while getting questions for topic {topicId}");
            return StatusCode(500, "An error occurred while retrieving questions by topic");
        }
    }
}