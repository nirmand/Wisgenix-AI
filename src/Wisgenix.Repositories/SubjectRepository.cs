using Wisgenix.Common.Exceptions;
using Wisgenix.Core.Logger;
using Wisgenix.Data;
using Wisgenix.Data.Entities;
using Wisgenix.Repositories.Base;
using Wisgenix.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Wisgenix.Repositories
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
                results => $"Successfully retrieved {results.Count()} subjects");
        }

        public async Task<Subject?> GetByIdAsync(LogContext logContext, int id)
        {
            return await ExecuteWithLoggingAsync(
                logContext,
                $"Retrieving subject with ID: {id}",
                async () =>
                {
                    var subject = await Context.Subjects.FindAsync(id) ?? throw new SubjectNotFoundException(id);
                    await Context.Entry(subject).Collection(s => s.Topics).LoadAsync();
                    return subject;
                },
                subject => $"Successfully retrieved subject with ID: {subject.ID}");
        }

        public async Task<Subject> CreateAsync(LogContext logContext, Subject subject)
        {
            // Check for duplicate subject name
            bool exists = await Context.Subjects.AnyAsync(s => s.SubjectName == subject.SubjectName);
            if (exists)
            {
                throw new DuplicateSubjectException(subject.SubjectName);
            }
            subject.CreatedDate = DateTime.UtcNow;
            subject.CreatedBy = logContext.UserName ?? "system";
            subject.ModifiedDate = null;
            subject.ModifiedBy = null;
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
            // Check for duplicate subject name (excluding current entity)
            bool exists = await Context.Subjects.AnyAsync(s => s.SubjectName == subject.SubjectName && s.ID != subject.ID);
            if (exists)
            {
                throw new DuplicateSubjectException(subject.SubjectName);
            }
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
                    existingSubject.ModifiedDate = DateTime.UtcNow;
                    existingSubject.ModifiedBy = logContext.UserName ?? "system";
                    await Context.SaveChangesAsync();
                    return existingSubject;
                },
                result => $"Successfully updated subject with ID: {result.ID}");
        }

        public async Task DeleteAsync(LogContext logContext, int id)
        {
            await ExecuteWithLoggingAsync(
                logContext,
                $"Deleting subject with ID: {id}",
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

        public async Task<bool> SubjectExistsAsync(LogContext logContext, int subjectId)
        {
            return await ExecuteWithLoggingAsync(
                logContext,
                $"Checking if subject exists with ID: {subjectId}",
                async () => await Context.Subjects.AnyAsync(s => s.ID == subjectId));
        }
    }
}