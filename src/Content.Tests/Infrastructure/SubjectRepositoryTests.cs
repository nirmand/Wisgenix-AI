using Microsoft.EntityFrameworkCore;
using Content.Domain.Entities;
using Content.Infrastructure.Data;
using Content.Infrastructure.Repositories;
using Xunit;

namespace Content.Tests.Infrastructure;

public class SubjectRepositoryTests : IDisposable
{
    private readonly ContentDbContext _context;
    private readonly SubjectRepository _repository;

    public SubjectRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ContentDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ContentDbContext(options);
        _repository = new SubjectRepository(_context);
    }

    [Fact]
    public async Task AddAsync_WithValidSubject_ShouldAddSubject()
    {
        // Arrange
        var subject = new Subject("Mathematics");

        // Act
        var result = await _repository.AddAsync(subject);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Mathematics", result.SubjectName);
        
        var savedSubject = await _context.Subjects.FirstOrDefaultAsync();
        Assert.NotNull(savedSubject);
        Assert.Equal("Mathematics", savedSubject.SubjectName);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnSubject()
    {
        // Arrange
        var subject = new Subject("Physics");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(subject.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Physics", result.SubjectName);
        Assert.Equal(subject.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_WithExistingName_ShouldReturnSubject()
    {
        // Arrange
        var subject = new Subject("Chemistry");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync("Chemistry");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Chemistry", result.SubjectName);
        Assert.Equal(subject.Id, result.Id);
    }

    [Fact]
    public async Task GetByNameAsync_WithNonExistingName_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByNameAsync("NonExisting");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsByNameAsync_WithExistingName_ShouldReturnTrue()
    {
        // Arrange
        var subject = new Subject("Biology");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByNameAsync("Biology");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByNameAsync_WithNonExistingName_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsByNameAsync("NonExisting");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsByNameAsync_WithExcludeId_ShouldExcludeSpecifiedId()
    {
        // Arrange
        var subject1 = new Subject("History");
        var subject2 = new Subject("Geography");
        _context.Subjects.AddRange(subject1, subject2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByNameAsync("History", subject1.Id);

        // Assert
        Assert.False(result); // Should return false because we're excluding the subject with this name
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleSubjects_ShouldReturnAllSubjects()
    {
        // Arrange
        var subject1 = new Subject("Math");
        var subject2 = new Subject("Science");
        var subject3 = new Subject("English");
        _context.Subjects.AddRange(subject1, subject2, subject3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count());
        Assert.Contains(result, s => s.SubjectName == "Math");
        Assert.Contains(result, s => s.SubjectName == "Science");
        Assert.Contains(result, s => s.SubjectName == "English");
    }

    [Fact]
    public async Task UpdateAsync_WithValidSubject_ShouldUpdateSubject()
    {
        // Arrange
        var subject = new Subject("Original Name");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        // Act
        subject.UpdateSubjectName("Updated Name");
        var result = await _repository.UpdateAsync(subject);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.SubjectName);
        
        var updatedSubject = await _context.Subjects.FindAsync(subject.Id);
        Assert.NotNull(updatedSubject);
        Assert.Equal("Updated Name", updatedSubject.SubjectName);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldDeleteSubject()
    {
        // Arrange
        var subject = new Subject("To Delete");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();
        var subjectId = subject.Id;

        // Act
        await _repository.DeleteAsync(subjectId);
        await _context.SaveChangesAsync();

        // Assert
        var deletedSubject = await _context.Subjects.FindAsync(subjectId);
        Assert.Null(deletedSubject);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
