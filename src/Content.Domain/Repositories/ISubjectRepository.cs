using Wisgenix.SharedKernel.Application;
using Content.Domain.Entities;

namespace Content.Domain.Repositories;

/// <summary>
/// Repository interface for Subject aggregate
/// </summary>
public interface ISubjectRepository : IRepository<Subject>
{
    Task<Subject?> GetByNameAsync(string subjectName, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subject>> GetSubjectsWithTopicsAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string subjectName, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string subjectName, int excludeId, CancellationToken cancellationToken = default);
}
