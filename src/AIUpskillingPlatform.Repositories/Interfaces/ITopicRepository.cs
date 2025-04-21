using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Core.Logger;

namespace AIUpskillingPlatform.Repositories.Interfaces;

public interface ITopicRepository
{
    Task<IEnumerable<Topic>> GetAllAsync(LogContext logContext);
    Task<Topic?> GetByIdAsync(LogContext logContext, int id);
    Task<Topic> CreateAsync(LogContext logContext, Topic topic);
    Task<Topic> UpdateAsync(LogContext logContext, Topic topic);
    Task DeleteAsync(LogContext logContext, int id);
    Task<bool> SubjectExistsAsync(LogContext logContext, int subjectId);
} 