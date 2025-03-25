using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Data;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AIUpskillingPlatform.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<QuestionRepository> _logger;

    public QuestionRepository(AppDbContext context, ILogger<QuestionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Question>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all questions");
            var questions = await _context.Questions
                .Include(q => q.Topic)
                .ToListAsync();
            _logger.LogInformation("Successfully retrieved {Count} questions", questions.Count);
            return questions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all questions");
            throw;
        }
    }

    public async Task<Question?> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving question with ID: {Id}", id);
            var question = await _context.Questions
                .Include(q => q.Topic)
                .FirstOrDefaultAsync(q => q.ID == id);
            
            if (question == null)
            {
                _logger.LogWarning("Question with ID: {Id} was not found", id);
                throw new QuestionNotFoundException(id);
            }
            
            _logger.LogInformation("Successfully retrieved question with ID: {Id}", id);
            return question;
        }
        catch (QuestionNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving question with ID: {Id}", id);
            throw;
        }
    }

    public async Task<Question> CreateAsync(Question question)
    {
        try
        {
            _logger.LogInformation("Creating new question for topic ID: {TopicId}", question.TopicID);
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully created question with ID: {Id}", question.ID);
            return question;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating question for topic ID: {TopicId}", question.TopicID);
            throw;
        }
    }

    public async Task<Question> UpdateAsync(Question question)
    {
        try
        {
            _logger.LogInformation("Updating question with ID: {Id}", question.ID);
            var existingQuestion = await _context.Questions.FindAsync(question.ID);
            if (existingQuestion == null)
            {
                _logger.LogWarning("Question with ID: {Id} was not found for update", question.ID);
                throw new QuestionNotFoundException(question.ID);
            }

            _context.Entry(existingQuestion).CurrentValues.SetValues(question);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated question with ID: {Id}", question.ID);
            return existingQuestion;
        }
        catch (QuestionNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating question with ID: {Id}", question.ID);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting question with ID: {Id}", id);
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                _logger.LogWarning("Question with ID: {Id} was not found for deletion", id);
                throw new QuestionNotFoundException(id);
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully deleted question with ID: {Id}", id);
        }
        catch (QuestionNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting question with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> TopicExistsAsync(int topicId)
    {
        try
        {
            return await _context.Topics.AnyAsync(t => t.ID == topicId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking if topic with ID: {TopicId} exists", topicId);
            throw;
        }
    }
} 