using Microsoft.EntityFrameworkCore;
using Content.Domain.Entities;
using Content.Domain.Repositories;
using Content.Infrastructure.Data;

namespace Content.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for QuestionOption entity
/// </summary>
public class QuestionOptionRepository : BaseRepository<QuestionOption>, IQuestionOptionRepository
{
    public QuestionOptionRepository(ContentDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<QuestionOption>> GetByQuestionIdAsync(int questionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(o => o.QuestionId == questionId)
            .Include(o => o.Question)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<QuestionOption>> GetCorrectOptionsByQuestionIdAsync(int questionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(o => o.QuestionId == questionId && o.IsCorrect)
            .Include(o => o.Question)
            .ToListAsync(cancellationToken);
    }

    public override async Task<QuestionOption?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Question)
            .ThenInclude(q => q.Topic)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<QuestionOption>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Question)
            .ThenInclude(q => q.Topic)
            .ToListAsync(cancellationToken);
    }
}
