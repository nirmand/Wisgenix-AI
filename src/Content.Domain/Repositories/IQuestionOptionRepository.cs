using Wisgenix.SharedKernel.Application;
using Content.Domain.Entities;

namespace Content.Domain.Repositories;

/// <summary>
/// Repository interface for QuestionOption entity
/// </summary>
public interface IQuestionOptionRepository : IRepository<QuestionOption>
{
    Task<IEnumerable<QuestionOption>> GetByQuestionIdAsync(int questionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuestionOption>> GetCorrectOptionsByQuestionIdAsync(int questionId, CancellationToken cancellationToken = default);
}
