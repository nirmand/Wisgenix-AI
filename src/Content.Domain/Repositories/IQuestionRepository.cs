using Wisgenix.SharedKernel.Application;
using Content.Domain.Entities;

namespace Content.Domain.Repositories;

/// <summary>
/// Repository interface for Question entity
/// </summary>
public interface IQuestionRepository : IRepository<Question>
{
    Task<IEnumerable<Question>> GetByTopicIdAsync(int topicId, CancellationToken cancellationToken = default);
    Task<Question?> GetByTextAndTopicAsync(string questionText, int topicId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Question>> GetQuestionsWithOptionsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Question>> GetByDifficultyLevelAsync(int difficultyLevel, CancellationToken cancellationToken = default);
    Task<bool> ExistsByTextAndTopicAsync(string questionText, int topicId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByTextAndTopicAsync(string questionText, int topicId, int excludeId, CancellationToken cancellationToken = default);
}
