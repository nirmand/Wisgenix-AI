using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Data;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AIUpskillingPlatform.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<TopicRepository> _logger;

    public TopicRepository(AppDbContext context, ILogger<TopicRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Topic>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all topics");
            var topics = await _context.Topics.ToListAsync();
            _logger.LogInformation("Successfully retrieved {Count} topics", topics.Count);
            return topics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all topics");
            throw;
        }
    }

    public async Task<Topic?> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving topic with ID: {Id}", id);
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                _logger.LogWarning("Topic with ID: {Id} was not found", id);
                throw new TopicNotFoundException(id);
            }
            _logger.LogInformation("Successfully retrieved topic with ID: {Id}", id);
            return topic;
        }
        catch (TopicNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving topic with ID: {Id}", id);
            throw;
        }
    }

    public async Task<Topic> CreateAsync(Topic topic)
    {
        try
        {
            _logger.LogInformation("Creating new topic with name: {TopicName}", topic.TopicName);
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully created topic with ID: {Id}", topic.ID);
            return topic;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating topic with name: {TopicName}", topic.TopicName);
            throw;
        }
    }

    public async Task<Topic> UpdateAsync(Topic topic)
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

    public async Task DeleteAsync(int id)
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

    public async Task<bool> SubjectExistsAsync(int subjectId)
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