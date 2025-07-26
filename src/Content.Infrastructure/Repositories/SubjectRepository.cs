using Microsoft.EntityFrameworkCore;
using Content.Domain.Entities;
using Content.Domain.Repositories;
using Content.Infrastructure.Data;

namespace Content.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Subject aggregate
/// </summary>
public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(ContentDbContext context) : base(context)
    {
    }

    public async Task<Subject?> GetByNameAsync(string subjectName, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(s => s.SubjectName == subjectName, cancellationToken);
    }

    public async Task<IEnumerable<Subject>> GetSubjectsWithTopicsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Topics)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string subjectName, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(s => s.SubjectName == subjectName, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string subjectName, int excludeId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(s => s.SubjectName == subjectName && s.Id != excludeId, cancellationToken);
    }

    public override async Task<Subject?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Topics)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Subject>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Topics)
            .ToListAsync(cancellationToken);
    }
}
