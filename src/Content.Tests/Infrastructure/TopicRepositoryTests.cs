using Microsoft.EntityFrameworkCore;
using Content.Domain.Entities;
using Content.Infrastructure.Data;
using Content.Infrastructure.Repositories;
using Xunit;

namespace Content.Tests.Infrastructure;

public class TopicRepositoryTests : IDisposable
{
    private readonly ContentDbContext _context;
    private readonly TopicRepository _repository;

    public TopicRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ContentDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ContentDbContext(options);
        _repository = new TopicRepository(_context);
    }

    [Fact]
    public async Task AddAsync_WithValidTopic_ShouldAddTopic()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("Algebra", subject.Id);

        // Act
        var result = await _repository.AddAsync(topic);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Algebra", result.TopicName);
        Assert.Equal(subject.Id, result.SubjectId);
        
        var savedTopic = await _context.Topics.FirstOrDefaultAsync();
        Assert.NotNull(savedTopic);
        Assert.Equal("Algebra", savedTopic.TopicName);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnTopic()
    {
        // Arrange
        var subject = new Subject("Physics");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("Mechanics", subject.Id);
        _context.Topics.Add(topic);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(topic.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Mechanics", result.TopicName);
        Assert.Equal(subject.Id, result.SubjectId);
        Assert.NotNull(result.Subject);
        Assert.Equal("Physics", result.Subject.SubjectName);
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
    public async Task GetBySubjectIdAsync_WithExistingSubjectId_ShouldReturnTopics()
    {
        // Arrange
        var subject = new Subject("Chemistry");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var topic1 = new Topic("Organic Chemistry", subject.Id);
        var topic2 = new Topic("Inorganic Chemistry", subject.Id);
        _context.Topics.AddRange(topic1, topic2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySubjectIdAsync(subject.Id);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, t => t.TopicName == "Organic Chemistry");
        Assert.Contains(result, t => t.TopicName == "Inorganic Chemistry");
    }

    [Fact]
    public async Task GetByNameAndSubjectAsync_WithExistingNameAndSubject_ShouldReturnTopic()
    {
        // Arrange
        var subject = new Subject("Biology");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("Genetics", subject.Id);
        _context.Topics.Add(topic);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAndSubjectAsync("Genetics", subject.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Genetics", result.TopicName);
        Assert.Equal(subject.Id, result.SubjectId);
    }

    [Fact]
    public async Task GetByNameAndSubjectAsync_WithNonExistingName_ShouldReturnNull()
    {
        // Arrange
        var subject = new Subject("History");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAndSubjectAsync("NonExisting", subject.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsByNameAndSubjectAsync_WithExistingNameAndSubject_ShouldReturnTrue()
    {
        // Arrange
        var subject = new Subject("Geography");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("World Geography", subject.Id);
        _context.Topics.Add(topic);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByNameAndSubjectAsync("World Geography", subject.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByNameAndSubjectAsync_WithNonExistingName_ShouldReturnFalse()
    {
        // Arrange
        var subject = new Subject("Art");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByNameAndSubjectAsync("NonExisting", subject.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsByNameAndSubjectAsync_WithExcludeId_ShouldExcludeSpecifiedId()
    {
        // Arrange
        var subject = new Subject("Literature");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var topic1 = new Topic("Poetry", subject.Id);
        var topic2 = new Topic("Prose", subject.Id);
        _context.Topics.AddRange(topic1, topic2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByNameAndSubjectAsync("Poetry", subject.Id, topic1.Id);

        // Assert
        Assert.False(result); // Should return false because we're excluding the topic with this name
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleTopics_ShouldReturnAllTopics()
    {
        // Arrange
        var subject1 = new Subject("Math");
        var subject2 = new Subject("Science");
        _context.Subjects.AddRange(subject1, subject2);
        await _context.SaveChangesAsync();

        var topic1 = new Topic("Calculus", subject1.Id);
        var topic2 = new Topic("Statistics", subject1.Id);
        var topic3 = new Topic("Physics", subject2.Id);
        _context.Topics.AddRange(topic1, topic2, topic3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count());
        Assert.Contains(result, t => t.TopicName == "Calculus");
        Assert.Contains(result, t => t.TopicName == "Statistics");
        Assert.Contains(result, t => t.TopicName == "Physics");
        Assert.All(result, t => Assert.NotNull(t.Subject));
    }

    [Fact]
    public async Task UpdateAsync_WithValidTopic_ShouldUpdateTopic()
    {
        // Arrange
        var subject = new Subject("Computer Science");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("Programming", subject.Id);
        _context.Topics.Add(topic);
        await _context.SaveChangesAsync();

        // Act
        topic.UpdateTopicName("Advanced Programming");
        var result = await _repository.UpdateAsync(topic);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Advanced Programming", result.TopicName);
        
        var updatedTopic = await _context.Topics.FindAsync(topic.Id);
        Assert.NotNull(updatedTopic);
        Assert.Equal("Advanced Programming", updatedTopic.TopicName);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldDeleteTopic()
    {
        // Arrange
        var subject = new Subject("Economics");
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("Microeconomics", subject.Id);
        _context.Topics.Add(topic);
        await _context.SaveChangesAsync();
        var topicId = topic.Id;

        // Act
        await _repository.DeleteAsync(topicId);
        await _context.SaveChangesAsync();

        // Assert
        var deletedTopic = await _context.Topics.FindAsync(topicId);
        Assert.Null(deletedTopic);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
