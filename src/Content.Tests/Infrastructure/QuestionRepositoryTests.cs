using Content.Domain.Entities;
using Content.Infrastructure.Data;
using Content.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Wisgenix.SharedKernel.Domain.Enums;
using Xunit;

namespace Content.Tests.Infrastructure;

public class QuestionRepositoryTests : IDisposable
{
    private readonly ContentDbContext _context;
    private readonly QuestionRepository _repository;
    private readonly string _dbName;

    public QuestionRepositoryTests()
    {
        _dbName = $"TestDb_{Guid.NewGuid()}.db";
        var options = new DbContextOptionsBuilder<ContentDbContext>()
            .UseSqlite($"Data Source={_dbName}")
            .Options;

        _context = new ContentDbContext(options);
        _context.Database.EnsureCreated(); // Create the database schema
        _repository = new QuestionRepository(_context);
    }

    [Fact]
    public async Task AddAsync_WithValidQuestion_ShouldAddQuestion()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("Algebra", subject.Id);
        await _context.Topics.AddAsync(topic);
        await _context.SaveChangesAsync();

        var question = new Question("What is 2 + 2?", topic.Id, 1, 5, QuestionSource.Manual);

        // Act
        var result = await _repository.AddAsync(question);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("What is 2 + 2?", result.QuestionText);
        Assert.Equal(topic.Id, result.TopicId);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnQuestion()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("Algebra", subject.Id);
        await _context.Topics.AddAsync(topic);
        await _context.SaveChangesAsync();

        var question = new Question("What is 2 + 2?", topic.Id, 1, 5, QuestionSource.Manual);
        await _context.Questions.AddAsync(question);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(question.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(question.Id, result.Id);
        Assert.Equal("What is 2 + 2?", result.QuestionText);
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
    public async Task GetAllAsync_WithQuestions_ShouldReturnAllQuestions()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("Algebra", subject.Id);
        await _context.Topics.AddAsync(topic);
        await _context.SaveChangesAsync();

        var question1 = new Question("What is 2 + 2?", topic.Id, 1, 5, QuestionSource.Manual);
        var question2 = new Question("What is 3 + 3?", topic.Id, 2, 7, QuestionSource.AI);
        await _context.Questions.AddRangeAsync(question1, question2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, q => q.QuestionText == "What is 2 + 2?");
        Assert.Contains(result, q => q.QuestionText == "What is 3 + 3?");
    }

    [Fact]
    public async Task GetByTopicIdAsync_WithExistingTopicId_ShouldReturnQuestions()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();

        var topic1 = new Topic("Algebra", subject.Id);
        var topic2 = new Topic("Geometry", subject.Id);
        await _context.Topics.AddRangeAsync(topic1, topic2);
        await _context.SaveChangesAsync();

        var question1 = new Question("What is 2 + 2?", topic1.Id, 1, 5, QuestionSource.Manual);
        var question2 = new Question("What is 3 + 3?", topic1.Id, 2, 7, QuestionSource.AI);
        var question3 = new Question("What is a triangle?", topic2.Id, 1, 5, QuestionSource.Manual);
        await _context.Questions.AddRangeAsync(question1, question2, question3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTopicIdAsync(topic1.Id);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, q => Assert.Equal(topic1.Id, q.TopicId));
        Assert.Contains(result, q => q.QuestionText == "What is 2 + 2?");
        Assert.Contains(result, q => q.QuestionText == "What is 3 + 3?");
    }

    [Fact]
    public async Task ExistsByTextAndTopicAsync_WithExistingQuestion_ShouldReturnTrue()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("Algebra", subject.Id);
        await _context.Topics.AddAsync(topic);
        await _context.SaveChangesAsync();

        var question = new Question("What is 2 + 2?", topic.Id, 1, 5, QuestionSource.Manual);
        await _context.Questions.AddAsync(question);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByTextAndTopicAsync("What is 2 + 2?", topic.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByTextAndTopicAsync_WithNonExistingQuestion_ShouldReturnFalse()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("Algebra", subject.Id);
        await _context.Topics.AddAsync(topic);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByTextAndTopicAsync("What is 2 + 2?", topic.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("Algebra", subject.Id);
        await _context.Topics.AddAsync(topic);
        await _context.SaveChangesAsync();

        var question = new Question("What is 2 + 2?", topic.Id, 1, 5, QuestionSource.Manual);
        await _context.Questions.AddAsync(question);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(question.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingId_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_WithValidQuestion_ShouldUpdateQuestion()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("Algebra", subject.Id);
        await _context.Topics.AddAsync(topic);
        await _context.SaveChangesAsync();

        var question = new Question("What is 2 + 2?", topic.Id, 1, 5, QuestionSource.Manual);
        await _context.Questions.AddAsync(question);
        await _context.SaveChangesAsync();

        // Act
        question.UpdateQuestion("What is 3 + 3?", 2, 7, QuestionSource.AI);
        var result = await _repository.UpdateAsync(question);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("What is 3 + 3?", result.QuestionText);
        Assert.Equal(2, result.DifficultyLevel);
        Assert.Equal(7, result.MaxScore);
        Assert.Equal(QuestionSource.AI, result.GeneratedBy);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldDeleteQuestion()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("Algebra", subject.Id);
        await _context.Topics.AddAsync(topic);
        await _context.SaveChangesAsync();

        var question = new Question("What is 2 + 2?", topic.Id, 1, 5, QuestionSource.Manual);
        await _context.Questions.AddAsync(question);
        await _context.SaveChangesAsync();

        var questionId = question.Id;

        // Act
        await _repository.DeleteAsync(questionId);
        await _context.SaveChangesAsync();

        // Assert
        var deletedQuestion = await _context.Questions.FindAsync(questionId);
        Assert.Null(deletedQuestion);
    }

    public void Dispose()
    {
        _context.Dispose();
        
        // Clean up test database file
        if (File.Exists(_dbName))
        {
            try
            {
                File.Delete(_dbName);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
