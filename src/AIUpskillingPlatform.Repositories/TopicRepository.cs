using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Data;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using AIUpskillingPlatform.Core.Logger;

namespace AIUpskillingPlatform.Repositories;

public class TopicRepository : BaseRepository<Topic>, ITopicRepository
{
    public TopicRepository(AppDbContext context, ILoggingService logger): base(context, logger)
    {
    }

    public async Task<IEnumerable<Topic>> GetAllAsync(LogContext logContext)
    {
        return await ExecuteWithLoggingAsync(
            logContext,
            "Retrieving all topics",
            async () => await Context.Topics.ToListAsync(),
            results => $"Successfully retrieved {results.Count()} topics");
    }

    public async Task<Topic?> GetByIdAsync(LogContext logContext, int id)
    {
        return await ExecuteWithLoggingAsync(
            logContext,
            $"Retrieving topic with ID: {id}",
            async () =>
            {
                var topic = await Context.Topics.FindAsync(id);
                if (topic == null)
                {
                    throw new TopicNotFoundException(id);
                }
                return topic;
            },
            topic => $"Successfully retrieved topic with ID: {topic.ID}");
    }

    public async Task<Topic> CreateAsync(LogContext logContext, Topic topic)
    {
        return await ExecuteWithLoggingAsync(
            logContext,
            $"Creating new topic",
            async () =>
            {
                Context.Topics.Add(topic);
                await Context.SaveChangesAsync();
                return topic;
            },
            result => $"Successfully created topic with ID: {result.ID}");
    }

    public async Task<Topic> UpdateAsync(LogContext logContext, Topic topic)
    {
        return await ExecuteWithLoggingAsync(
            logContext,
            $"Updating topic with ID: {topic.ID}",
            async () =>
            {
                var existingTopic = await Context.Topics.FindAsync(topic.ID);
                if (existingTopic == null)
                {
                    throw new TopicNotFoundException(topic.ID);
                }
                Context.Entry(existingTopic).CurrentValues.SetValues(topic);
                await Context.SaveChangesAsync();
                return existingTopic;
            },
            result => $"Successfully updated topic with ID: {result.ID}");
    }

    public async Task DeleteAsync(LogContext logContext, int id)
    {
        await ExecuteWithLoggingAsync(
            logContext,
            $"Deleting topic with ID: {id}",
            async () =>
            {
                var topic = await Context.Topics.FindAsync(id);
                if (topic == null)
                {
                    throw new TopicNotFoundException(id);
                }
                Context.Topics.Remove(topic);
                await Context.SaveChangesAsync();
            });
    }

    public async Task<bool> SubjectExistsAsync(LogContext logContext, int subjectId)
    {
        return await ExecuteWithLoggingAsync(
            logContext,
            $"Checking if subject exists with ID: {subjectId}",
            async () => await Context.Subjects.AnyAsync(s => s.ID == subjectId));
    }
} 