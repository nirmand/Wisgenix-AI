using Wisgenix.Common.Exceptions;
using Wisgenix.Core.Logger;
using Wisgenix.Data;
using Wisgenix.Data.Entities;
using Wisgenix.Repositories.Base;
using Wisgenix.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Wisgenix.Repositories;

public class QuestionOptionRepository : BaseRepository<QuestionOption>, IQuestionOptionRepository
{
    public QuestionOptionRepository(AppDbContext context, ILoggingService logger) : base(context, logger)
    {
    }

    public async Task<IEnumerable<QuestionOption>> GetAllAsync(LogContext logContext)
    {
        return await ExecuteWithLoggingAsync(
            logContext,
            "Retrieving all question options",
            async () => await Context.QuestionOptions.Include(o => o.Question).ToListAsync(),
            results => $"Successfully retrieved {results.Count()} question options");
    }

    public async Task<QuestionOption?> GetByIdAsync(LogContext logContext, int id)
    {
        return await ExecuteWithLoggingAsync(
            logContext,
            $"Retrieving question option with ID: {id}",
            async () =>
            {
                var option = await Context.QuestionOptions
                    .Include(o => o.Question)
                    .FirstOrDefaultAsync(o => o.ID == id) ?? throw new QuestionOptionNotFoundException(id);
                return option;
            },
            option => $"Successfully retrieved question option with ID: {option.ID}");
    }

    public async Task<QuestionOption> CreateAsync(LogContext logContext, QuestionOption option)
    {
        option.CreatedDate = DateTime.UtcNow;
        option.CreatedBy = logContext.UserName ?? "system";
        option.ModifiedDate = null;
        option.ModifiedBy = null;
        return await ExecuteWithLoggingAsync(
            logContext,
            $"Creating new question option",
            async () =>
            {
                Context.QuestionOptions.Add(option);
                await Context.SaveChangesAsync();
                return option;
            },
            result => $"Successfully created question option with ID: {result.ID}");
    }

    public async Task<QuestionOption> UpdateAsync(LogContext logContext, QuestionOption option)
    {
        return await ExecuteWithLoggingAsync(
            logContext,
            $"Updating question option with ID: {option.ID}",
            async () =>
            {
                var existingOption = await Context.QuestionOptions.FindAsync(option.ID);
                if (existingOption == null)
                {
                    throw new QuestionOptionNotFoundException(option.ID);
                }
                Context.Entry(existingOption).CurrentValues.SetValues(option);
                existingOption.ModifiedDate = DateTime.UtcNow;
                existingOption.ModifiedBy = logContext.UserName ?? "system";
                await Context.SaveChangesAsync();
                return existingOption;
            },
            result => $"Successfully updated question option with ID: {result.ID}");
    }

    public async Task DeleteAsync(LogContext logContext, int id)
    {
        await ExecuteWithLoggingAsync(
            logContext,
            $"Deleting question option with ID: {id}",
            async () =>
            {
                var option = await Context.QuestionOptions.FindAsync(id);
                if (option == null)
                {
                    throw new QuestionOptionNotFoundException(id);
                }
                Context.QuestionOptions.Remove(option);
                await Context.SaveChangesAsync();
            });
    }

    public async Task<bool> QuestionExistsAsync(LogContext logContext, int questionId)
    {
        return await ExecuteWithLoggingAsync(
            logContext,
            $"Checking if question exists with ID: {questionId}",
            async () => await Context.Questions.AnyAsync(q => q.ID == questionId));
    }

    public async Task<IEnumerable<QuestionOption>> GetByQuestionIdAsync(LogContext logContext, int questionId)
    {
        return await ExecuteWithLoggingAsync(
            logContext,
            $"Retrieving options for question ID: {questionId}",
            async () => await Context.QuestionOptions
                .Where(o => o.QuestionID == questionId)
                .Include(o => o.Question)
                .ToListAsync(),
            results => $"Successfully retrieved {results.Count()} options for question ID: {questionId}");
    }
}