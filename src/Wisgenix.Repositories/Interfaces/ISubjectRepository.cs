using Wisgenix.Core.Logger;
using Wisgenix.Data.Entities;

namespace Wisgenix.Repositories.Interfaces;

public interface ISubjectRepository
{
    Task<IEnumerable<Subject>> GetAllAsync(LogContext logContext);
    Task<Subject?> GetByIdAsync(LogContext logContext, int id);
    Task<Subject> CreateAsync(LogContext logContext, Subject subject);
    Task<Subject> UpdateAsync(LogContext logContext, Subject subject);
    Task DeleteAsync(LogContext logContext, int id);
    Task<bool> SubjectExistsAsync(LogContext logContext, int subjectId);
}