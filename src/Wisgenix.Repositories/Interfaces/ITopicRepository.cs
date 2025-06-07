using Wisgenix.Data.Entities;
using Wisgenix.Core.Logger;

namespace Wisgenix.Repositories.Interfaces;

public interface ITopicRepository
{
    Task<IEnumerable<Topic>> GetAllAsync(LogContext logContext);
    Task<Topic?> GetByIdAsync(LogContext logContext, int id);
    Task<Topic> CreateAsync(LogContext logContext, Topic topic);
    Task<Topic> UpdateAsync(LogContext logContext, Topic topic);
    Task DeleteAsync(LogContext logContext, int id);
    Task<bool> SubjectExistsAsync(LogContext logContext, int subjectId);
    Task<IEnumerable<Topic>> GetBySubjectIdAsync(LogContext logContext, int subjectId);
}