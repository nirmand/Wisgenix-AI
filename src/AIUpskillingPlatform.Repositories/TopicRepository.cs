using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Data;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using AIUpskillingPlatform.Core.Logger;

namespace AIUpskillingPlatform.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly AppDbContext _context;
    private readonly ILoggingService _logger;

    public TopicRepository(AppDbContext context, ILoggingService logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Topic>> GetAllAsync(LogContext logContext)
    {
        try
        {
            _logger.LogOperationStart<Topic>(logContext, "Retrieving all topics");
            var topics = await _context.Topics.ToListAsync();
            _logger.LogOperationSuccess<Topic>(logContext, $"Successfully retrieved {topics.Count} topics");
            return topics;
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, "Error occurred while retrieving all topics");
            throw;
        }
    }

    public async Task<Topic?> GetByIdAsync(LogContext logContext, int id)
    {
        try
        {
            _logger.LogOperationStart<Topic>(logContext, $"Retrieving topic with ID: {id}");
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                throw new TopicNotFoundException(id);
            }
            _logger.LogOperationSuccess<Topic>(logContext, $"Successfully retrieved topic with ID: {id}");
            return topic;
        }
        catch (TopicNotFoundException ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, $"Topic with ID: {id} not found");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, $"Failed to retrieve topic {id}");
            throw;
        }
    }

    public async Task<Topic> CreateAsync(LogContext logContext, Topic topic)
    {
        try
        {
            _logger.LogOperationStart<Topic>(logContext, $"Creating new topic with name: {topic.TopicName}");
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();
            _logger.LogOperationSuccess<Topic>(logContext, $"Successfully created topic with ID: {topic.ID}");
            return topic;
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, $"Error occurred while creating topic with name: {topic.TopicName}");
            throw;
        }
    }

    public async Task<Topic> UpdateAsync(LogContext logContext, Topic topic)
    {
        try
        {
            _logger.LogInformation("Updating topic with ID: {Id}", topic.ID);
            var existingTopic = await _context.Topics.FindAsync(topic.ID);
            if (existingTopic == null)
            {
                _logger.LogWarning("Topic with ID: {Id} was not found for update", topic.ID);
                throw new TopicNotFoundException(topic.ID);
            }

            _context.Entry(existingTopic).CurrentValues.SetValues(topic);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated topic with ID: {Id}", topic.ID);
            return existingTopic;
        }
        catch (TopicNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating topic with ID: {Id}", topic.ID);
            throw;
        }
    }

    public async Task DeleteAsync(LogContext logContext, int id)
    {
        try
        {
            _logger.LogInformation("Deleting topic with ID: {Id}", id);
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                _logger.LogWarning("Topic with ID: {Id} was not found for deletion", id);
                throw new TopicNotFoundException(id);
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully deleted topic with ID: {Id}", id);
        }
        catch (TopicNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting topic with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> SubjectExistsAsync(LogContext logContext, int subjectId)
    {
        try
        {
            return await _context.Subjects.AnyAsync(s => s.ID == subjectId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking if subject exists with ID: {Id}", subjectId);
            throw;
        }
    }
} 