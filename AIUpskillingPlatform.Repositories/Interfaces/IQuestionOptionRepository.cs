using AIUpskillingPlatform.Data.Entities;

namespace AIUpskillingPlatform.Repositories.Interfaces;

public interface IQuestionOptionRepository
{
    Task<IEnumerable<QuestionOption>> GetAllAsync();
    Task<QuestionOption?> GetByIdAsync(int id);
    Task<QuestionOption> CreateAsync(QuestionOption questionOption);
    Task<QuestionOption> UpdateAsync(QuestionOption questionOption);
    Task DeleteAsync(int id);
    Task<bool> QuestionExistsAsync(int questionId);
    Task<IEnumerable<QuestionOption>> GetByQuestionIdAsync(int questionId);
} 