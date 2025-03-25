using AIUpskillingPlatform.Data.Entities;

namespace AIUpskillingPlatform.Repositories.Interfaces;

public interface ITopicRepository
{
    Task<IEnumerable<Topic>> GetAllAsync();
    Task<Topic?> GetByIdAsync(int id);
    Task<Topic> CreateAsync(Topic topic);
    Task<Topic> UpdateAsync(Topic topic);
    Task DeleteAsync(int id);
    Task<bool> SubjectExistsAsync(int subjectId);
} 