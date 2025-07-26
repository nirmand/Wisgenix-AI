using Microsoft.EntityFrameworkCore;
using Content.Domain.Entities;
using Content.Domain.Repositories;
using Content.Infrastructure.Data;

namespace Content.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Question entity
/// </summary>
public class QuestionRepository : BaseRepository<Question>, IQuestionRepository
{
    public QuestionRepository(ContentDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Question>> GetByTopicIdAsync(int topicId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(q => q.TopicId == topicId)
            .Include(q => q.Options)
            .Include(q => q.Topic)
            .ToListAsync(cancellationToken);
    }

    public async Task<Question?> GetByTextAndTopicAsync(string questionText, int topicId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(q => q.QuestionText == questionText && q.TopicId == topicId, cancellationToken);
    }

    public async Task<IEnumerable<Question>> GetQuestionsWithOptionsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(q => q.Options)
            .Include(q => q.Topic)
            .ThenInclude(t => t.Subject)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Question>> GetByDifficultyLevelAsync(int difficultyLevel, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(q => q.DifficultyLevel == difficultyLevel)
            .Include(q => q.Options)
            .Include(q => q.Topic)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByTextAndTopicAsync(string questionText, int topicId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(q => q.QuestionText == questionText && q.TopicId == topicId, cancellationToken);
    }

    public async Task<bool> ExistsByTextAndTopicAsync(string questionText, int topicId, int excludeId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(q => q.QuestionText == questionText && q.TopicId == topicId && q.Id != excludeId, cancellationToken);
    }

    public override async Task<Question?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(q => q.Options)
            .Include(q => q.Topic)
            .ThenInclude(t => t.Subject)
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Question>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(q => q.Options)
            .Include(q => q.Topic)
            .ThenInclude(t => t.Subject)
            .ToListAsync(cancellationToken);
    }
}
