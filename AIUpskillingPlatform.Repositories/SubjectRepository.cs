using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using AIUpskillingPlatform.Data;

namespace AIUpskillingPlatform.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SubjectRepository> _logger;

        public SubjectRepository(AppDbContext context, ILogger<SubjectRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            try
            {
                return await _context.Subjects.Include(s => s.Topics).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all subjects");
                throw;
            }
        }

        public async Task<Subject?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Subjects
                    .Include(s => s.Topics)
                    .FirstOrDefaultAsync(s => s.ID == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving subject with ID: {Id}", id);
                throw;
            }
        }

        public async Task<Subject> CreateAsync(Subject subject)
        {
            try
            {
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();
                return subject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating subject");
                throw;
            }
        }

        public async Task<Subject> UpdateAsync(Subject subject)
        {
            try
            {
                _context.Entry(subject).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return subject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating subject with ID: {Id}", subject.ID);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var subject = await _context.Subjects.FindAsync(id);
                if (subject != null)
                {
                    _context.Subjects.Remove(subject);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting subject with ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Subjects.AnyAsync(s => s.ID == id);
        }
    }
} 