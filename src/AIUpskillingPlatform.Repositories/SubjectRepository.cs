using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Core.Logger;
using AIUpskillingPlatform.Data;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Base;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AIUpskillingPlatform.Repositories
{
    public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(AppDbContext context, ILoggingService logger) : base(context, logger)
        {
        }

        public async Task<IEnumerable<Subject>> GetAllAsync(LogContext logContext)
        {
            return await ExecuteWithLoggingAsync(
                logContext,
                "Retrieving all subjects",
                async () => await Context.Subjects.Include(s => s.Topics).ToListAsync(),
                results => $"Successfully retrieved {results.Count} subjects");
        }

        public async Task<Subject?> GetByIdAsync(LogContext logContext, int id)
        {
            return await ExecuteWithLoggingAsync(
                logContext,
                $"Retrieving subject with ID: {id}",
                async () =>
                {
                    var subject = await Context.Subjects.Include(s => s.Topics).FirstOrDefaultAsync(s => s.ID == id) ?? throw new SubjectNotFoundException(id);
                    return subject;
                },
                subject => $"Successfully retrieved subject with ID: {subject.ID}");
        }

        public async Task<Subject> CreateAsync(LogContext logContext, Subject subject)
        {
            return await ExecuteWithLoggingAsync(
            logContext,
            $"Creating new subject",
            async () =>
            {
                Context.Subjects.Add(subject);
                await Context.SaveChangesAsync();
                return subject;
            },
            result => $"Successfully created subject with ID: {result.ID}");
        }

        public async Task<Subject> UpdateAsync(LogContext logContext, Subject subject)
        {
            return await ExecuteWithLoggingAsync(
            logContext,
            $"Updating subject with ID: {subject.ID}",
            async () =>
            {
                var existingSubject = await Context.Subjects.FindAsync(subject.ID);
                if (existingSubject == null)
                {
                    throw new SubjectNotFoundException(subject.ID);
                }
                Context.Entry(existingSubject).CurrentValues.SetValues(subject);
                await Context.SaveChangesAsync();
                return existingSubject;
            },
            result => $"Successfully updated subject with ID: {result.ID}");
        }

        public async Task DeleteAsync(LogContext logContext, int id)
        {
            await ExecuteWithLoggingAsync(
            logContext,
            $"Deleting topic with ID: {id}",
            async () =>
            {
                var subject = await Context.Subjects.FindAsync(id);
                if (subject == null)
                {
                    throw new SubjectNotFoundException(id);
                }
                Context.Subjects.Remove(subject);
                await Context.SaveChangesAsync();
            });
        }

        public async Task<bool> CheckIfExists(LogContext logContext,int id)
        {
            return await ExecuteWithLoggingAsync(
            logContext,
            $"Checking if subject exists with ID: {id}",
            async () => await Context.Subjects.AnyAsync(s => s.ID == id));
        }
    }
}