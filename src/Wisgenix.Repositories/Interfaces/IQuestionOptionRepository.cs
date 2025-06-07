using Wisgenix.Core.Logger;
using Wisgenix.Data.Entities;

namespace Wisgenix.Repositories.Interfaces;

public interface IQuestionOptionRepository
{
    Task<IEnumerable<QuestionOption>> GetAllAsync(LogContext logContext);
    Task<QuestionOption?> GetByIdAsync(LogContext logContext, int id);
    Task<QuestionOption> CreateAsync(LogContext logContext, QuestionOption questionOption);
    Task<QuestionOption> UpdateAsync(LogContext logContext, QuestionOption questionOption);
    Task DeleteAsync(LogContext logContext, int id);
    Task<bool> QuestionExistsAsync(LogContext logContext, int questionId);
    Task<IEnumerable<QuestionOption>> GetByQuestionIdAsync(LogContext logContext, int questionId);
}