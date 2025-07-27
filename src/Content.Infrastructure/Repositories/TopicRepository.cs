using Microsoft.EntityFrameworkCore;
using Content.Domain.Entities;
using Content.Domain.Repositories;
using Content.Infrastructure.Data;

namespace Content.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Topic entity
/// </summary>
public class TopicRepository : BaseRepository<Topic>, ITopicRepository
{
    public TopicRepository(ContentDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Topic>> GetBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.SubjectId == subjectId)
            .Include(t => t.Questions)
            .ToListAsync(cancellationToken);
    }

    public async Task<Topic?> GetByNameAndSubjectAsync(string topicName, int subjectId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(t => t.TopicName == topicName && t.SubjectId == subjectId, cancellationToken);
    }

    public async Task<IEnumerable<Topic>> GetTopicsWithQuestionsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Questions)
            .ThenInclude(q => q.Options)
            .Include(t => t.Subject)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAndSubjectAsync(string topicName, int subjectId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(t => t.TopicName == topicName && t.SubjectId == subjectId, cancellationToken);
    }

    public async Task<bool> ExistsByNameAndSubjectAsync(string topicName, int subjectId, int excludeId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(t => t.TopicName == topicName && t.SubjectId == subjectId && t.Id != excludeId, cancellationToken);
    }

    public override async Task<Topic?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Questions)
            .ThenInclude(q => q.Options)
            .Include(t => t.Subject)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Topic>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Subject)
            .Include(t => t.Questions)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Topic>> GetTopicsWithQuestionsBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Subject)
            .Include(t => t.Questions)
            .ThenInclude(q => q.Options)
            .Where(t => t.SubjectId == subjectId)
            .ToListAsync(cancellationToken);
    }
}
