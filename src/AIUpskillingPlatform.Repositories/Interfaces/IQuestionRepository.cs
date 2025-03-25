using AIUpskillingPlatform.Data.Entities;

namespace AIUpskillingPlatform.Repositories.Interfaces;

public interface IQuestionRepository
{
    Task<IEnumerable<Question>> GetAllAsync();
    Task<Question?> GetByIdAsync(int id);
    Task<Question> CreateAsync(Question question);
    Task<Question> UpdateAsync(Question question);
    Task DeleteAsync(int id);
    Task<bool> TopicExistsAsync(int topicId);
} 