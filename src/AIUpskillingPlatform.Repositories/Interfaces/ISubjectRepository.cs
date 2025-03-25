using AIUpskillingPlatform.Data.Entities;

namespace AIUpskillingPlatform.Repositories.Interfaces;
public interface ISubjectRepository
{
    Task<IEnumerable<Subject>> GetAllAsync();
    Task<Subject?> GetByIdAsync(int id);
    Task<Subject> CreateAsync(Subject subject);
    Task<Subject> UpdateAsync(Subject subject);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
} 