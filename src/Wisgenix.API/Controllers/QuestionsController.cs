using Wisgenix.Common.Exceptions;
using Wisgenix.Core.Logger;
using Wisgenix.Data.Entities;
using Wisgenix.DTO;
using Wisgenix.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace Wisgenix.API.Controllers;

[ApiController]
[Route("api/content")]
public class QuestionsController : BaseApiController
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ILoggingService _logger;
    private readonly IMapper _mapper;

    public QuestionsController(IQuestionRepository questionRepository, ILoggingService logger, IMapper mapper)
    {
        _questionRepository = questionRepository;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet("questions")]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestions()
    {
        var logContext = CreateLogContext("GetQuestions");
        try
        {
            var questions = await _questionRepository.GetAllAsync(logContext);
            var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);
            return Ok(questionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Question>(logContext, ex, "Error occurred while getting all questions");
            return StatusCode(500, "An error occurred while retrieving questions");
        }
    }

    [HttpGet("question/{id}")]
    public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
    {
        var logContext = CreateLogContext("GetQuestion");
        try
        {
            var question = await _questionRepository.GetByIdAsync(logContext, id);
            var questionDto = _mapper.Map<QuestionDto>(question);
            return Ok(questionDto);
        }
        catch (QuestionNotFoundException ex)
        {
            _logger.LogOperationWarning<Question>(logContext, ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Question>(logContext, ex, "Error occurred while getting question");
            return StatusCode(500, "An error occurred while retrieving the question");
        }
    }

    [HttpPost("create-question")]
    public async Task<ActionResult<QuestionDto>> CreateQuestion([FromBody] CreateQuestionDto createQuestionDto)
    {
        var userName = User?.Identity?.Name ?? "system";
        var logContext = CreateLogContext("CreateQuestion", userName);
        try
        {
            if (!await _questionRepository.TopicExistsAsync(logContext, createQuestionDto.TopicID))
            {
                _logger.LogOperationWarning<Question>(logContext, $"Topic with ID {createQuestionDto.TopicID} does not exist");
                return BadRequest($"Topic with ID {createQuestionDto.TopicID} does not exist");
            }
            var question = _mapper.Map<Question>(createQuestionDto);
            var createdQuestion = await _questionRepository.CreateAsync(logContext, question);
            var questionDto = _mapper.Map<QuestionDto>(createdQuestion);
            return CreatedAtAction(nameof(GetQuestion), new { id = createdQuestion.ID }, questionDto);
        }
        catch (DuplicateQuestionException ex)
        {
            _logger.LogOperationWarning<Question>(logContext, ex.Message);
            return BadRequest(new { message = ex.Message });
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
        var logContext = CreateLogContext("UpdateQuestion", userName);
        try
        {
            if (!await _questionRepository.TopicExistsAsync(logContext, updateQuestionDto.TopicID))
            {
                var msg = $"Topic with ID {updateQuestionDto.TopicID} does not exist";
                _logger.LogOperationWarning<Question>(logContext, msg);
                throw new TopicNotFoundException(updateQuestionDto.TopicID);
            }
            var question = _mapper.Map<Question>(updateQuestionDto);
            question.ID = id;
            await _questionRepository.UpdateAsync(logContext, question);
            return NoContent();
        }
        catch (TopicNotFoundException ex)
        {
            _logger.LogOperationWarning<Question>(logContext, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (DuplicateQuestionException ex)
        {
            _logger.LogOperationWarning<Question>(logContext, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (QuestionNotFoundException ex)
        {
            _logger.LogOperationWarning<Question>(logContext, ex.Message);
            return BadRequest(new { message = ex.Message });
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
        var logContext = CreateLogContext("DeleteQuestion");
        try
        {
            _logger.LogInformation($"Deleting question with ID: {id}");
            await _questionRepository.DeleteAsync(logContext, id);
            _logger.LogInformation($"Successfully deleted question with ID: {id}");
            return NoContent();
        }
        catch (QuestionNotFoundException ex)
        {
            _logger.LogOperationWarning<Question>(logContext, ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Question>(logContext, ex, "Error occurred while deleting question");
            return StatusCode(500, "An error occurred while deleting the question");
        }
    }

    [HttpGet("questions-by-topic/{topicId}")]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestionsByTopic(int topicId)
    {
        var logContext = CreateLogContext("GetQuestionsByTopic");
        try
        {
            var questions = await _questionRepository.GetByTopicIdAsync(logContext, topicId);
            var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);
            return Ok(questionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Question>(logContext, ex, $"Error occurred while getting questions for topic {topicId}");
            return StatusCode(500, "An error occurred while retrieving questions by topic");
        }
    }
}