using Wisgenix.Common.Exceptions;
using Wisgenix.Core.Logger;
using Wisgenix.Data;
using Wisgenix.Data.Entities;
using Wisgenix.Repositories.Base;
using Wisgenix.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Wisgenix.Repositories;

public class QuestionRepository : BaseRepository<Question>, IQuestionRepository
{
    public QuestionRepository(AppDbContext context, ILoggingService logger) : base(context, logger)
    {
    }

    public async Task<IEnumerable<Question>> GetAllAsync(LogContext logContext)
    {
        return await ExecuteWithLoggingAsync(
            logContext,
            "Retrieving all questions",
            async () => await Context.Questions.Include(q => q.Topic).ToListAsync(),
            results => $"Successfully retrieved {results.Count()} questions");
    }

    public async Task<Question?> GetByIdAsync(LogContext logContext, int id)
    {
        return await ExecuteWithLoggingAsync(
            logContext,
            $"Retrieving question with ID: {id}",
            async () =>
            {
                var question = await Context.Questions
                    .Include(q => q.Topic)
                    .FirstOrDefaultAsync(q => q.ID == id) ?? throw new QuestionNotFoundException(id);
                return question;
            },
            question => $"Successfully retrieved question with ID: {question.ID}");
    }

    public async Task<Question> CreateAsync(LogContext logContext, Question question)
    {
        // Check for duplicate question text for the same topic
        bool exists = await Context.Questions.AnyAsync(q => q.QuestionText == question.QuestionText && q.TopicID == question.TopicID);
        if (exists)
        {
            throw new DuplicateQuestionException(question.QuestionText, question.TopicID);
        }
        question.CreatedDate = DateTime.UtcNow;
        question.CreatedBy = logContext.UserName ?? "system";
        question.ModifiedDate = null;
        question.ModifiedBy = null;
        return await ExecuteWithLoggingAsync(
            logContext,
            $"Creating new question",
            async () =>
            {
                Context.Questions.Add(question);
                await Context.SaveChangesAsync();
                return question;
            },
            result => $"Successfully created question with ID: {result.ID}");
    }

    public async Task<Question> UpdateAsync(LogContext logContext, Question question)
    {
        // Check for duplicate question text for the same topic (excluding current entity)
        bool exists = await Context.Questions.AnyAsync(q => q.QuestionText == question.QuestionText && q.TopicID == question.TopicID && q.ID != question.ID);
        if (exists)
        {
            throw new DuplicateQuestionException(question.QuestionText, question.TopicID);
        }
        return await ExecuteWithLoggingAsync(
            logContext,
            $"Updating question with ID: {question.ID}",
            async () =>
            {
                var existingQuestion = await Context.Questions.FindAsync(question.ID);
                if (existingQuestion == null)
                {
                    throw new QuestionNotFoundException(question.ID);
                }
                Context.Entry(existingQuestion).CurrentValues.SetValues(question);
                existingQuestion.ModifiedDate = DateTime.UtcNow;
                existingQuestion.ModifiedBy = logContext.UserName ?? "system";
                await Context.SaveChangesAsync();
                return existingQuestion;
            },
            result => $"Successfully updated question with ID: {result.ID}");
    }

    public async Task DeleteAsync(LogContext logContext, int id)
    {
        await ExecuteWithLoggingAsync(
            logContext,
            $"Deleting question with ID: {id}",
            async () =>
            {
                var question = await Context.Questions.FindAsync(id);
                if (question == null)
                {
                    throw new QuestionNotFoundException(id);
                }
                Context.Questions.Remove(question);
                await Context.SaveChangesAsync();
            });
    }

    public async Task<bool> TopicExistsAsync(LogContext logContext, int topicId)
    {
        return await ExecuteWithLoggingAsync(
            logContext,
            $"Checking if topic exists with ID: {topicId}",
            async () => await Context.Topics.AnyAsync(t => t.ID == topicId));
    }

    public async Task<IEnumerable<Question>> GetByTopicIdAsync(LogContext logContext, int topicId)
    {
        return await ExecuteWithLoggingAsync(
            logContext,
            $"Retrieving questions for topic ID: {topicId}",
            async () => await Context.Questions
                .Include(q => q.Topic)
                .Where(q => q.TopicID == topicId)
                .ToListAsync(),
            results => $"Successfully retrieved {results.Count()} questions for topic ID: {topicId}");
    }
}