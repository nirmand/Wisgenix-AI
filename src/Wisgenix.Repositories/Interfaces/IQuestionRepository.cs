using Wisgenix.Core.Logger;
using Wisgenix.Data.Entities;

namespace Wisgenix.Repositories.Interfaces;

public interface IQuestionRepository
{
    Task<IEnumerable<Question>> GetAllAsync(LogContext logContext);
    Task<Question?> GetByIdAsync(LogContext logContext, int id);
    Task<Question> CreateAsync(LogContext logContext, Question question);
    Task<Question> UpdateAsync(LogContext logContext, Question question);
    Task DeleteAsync(LogContext logContext, int id);
    Task<bool> TopicExistsAsync(LogContext logContext, int topicId);
    Task<IEnumerable<Question>> GetByTopicIdAsync(LogContext logContext, int topicId);
}