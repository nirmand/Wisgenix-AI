using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Data;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AIUpskillingPlatform.Repositories;

public class QuestionOptionRepository : IQuestionOptionRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<QuestionOptionRepository> _logger;

    public QuestionOptionRepository(AppDbContext context, ILogger<QuestionOptionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<QuestionOption>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all question options");
            var options = await _context.QuestionOptions
                .Include(o => o.Question)
                .ToListAsync();
            _logger.LogInformation("Successfully retrieved {Count} question options", options.Count);
            return options;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all question options");
            throw;
        }
    }

    public async Task<QuestionOption?> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving question option with ID: {Id}", id);
            var option = await _context.QuestionOptions
                .Include(o => o.Question)
                .FirstOrDefaultAsync(o => o.ID == id);
            
            if (option == null)
            {
                _logger.LogWarning("Question option with ID: {Id} was not found", id);
                throw new QuestionOptionNotFoundException(id);
            }
            
            _logger.LogInformation("Successfully retrieved question option with ID: {Id}", id);
            return option;
        }
        catch (QuestionOptionNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving question option with ID: {Id}", id);
            throw;
        }
    }

    public async Task<QuestionOption> CreateAsync(QuestionOption questionOption)
    {
        try
        {
            _logger.LogInformation("Creating new question option for question ID: {QuestionId}", questionOption.QuestionID);
            _context.QuestionOptions.Add(questionOption);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully created question option with ID: {Id}", questionOption.ID);
            return questionOption;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating question option for question ID: {QuestionId}", questionOption.QuestionID);
            throw;
        }
    }

    public async Task<QuestionOption> UpdateAsync(QuestionOption questionOption)
    {
        try
        {
            _logger.LogInformation("Updating question option with ID: {Id}", questionOption.ID);
            var existingOption = await _context.QuestionOptions.FindAsync(questionOption.ID);
            if (existingOption == null)
            {
                _logger.LogWarning("Question option with ID: {Id} was not found for update", questionOption.ID);
                throw new QuestionOptionNotFoundException(questionOption.ID);
            }

            _context.Entry(existingOption).CurrentValues.SetValues(questionOption);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated question option with ID: {Id}", questionOption.ID);
            return existingOption;
        }
        catch (QuestionOptionNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating question option with ID: {Id}", questionOption.ID);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting question option with ID: {Id}", id);
            var option = await _context.QuestionOptions.FindAsync(id);
            if (option == null)
            {
                _logger.LogWarning("Question option with ID: {Id} was not found for deletion", id);
                throw new QuestionOptionNotFoundException(id);
            }

            _context.QuestionOptions.Remove(option);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully deleted question option with ID: {Id}", id);
        }
        catch (QuestionOptionNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting question option with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> QuestionExistsAsync(int questionId)
    {
        try
        {
            return await _context.Questions.AnyAsync(q => q.ID == questionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking if question with ID: {QuestionId} exists", questionId);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionOption>> GetByQuestionIdAsync(int questionId)
    {
        try
        {
            _logger.LogInformation("Retrieving options for question ID: {QuestionId}", questionId);
            var options = await _context.QuestionOptions
                .Where(o => o.QuestionID == questionId)
                .ToListAsync();
            _logger.LogInformation("Successfully retrieved {Count} options for question ID: {QuestionId}", options.Count, questionId);
            return options;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving options for question ID: {QuestionId}", questionId);
            throw;
        }
    }
} 