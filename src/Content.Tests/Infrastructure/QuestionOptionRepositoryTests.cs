using Content.Domain.Entities;
using Content.Infrastructure.Data;
using Content.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Wisgenix.SharedKernel.Domain.Enums;
using Xunit;

namespace Content.Tests.Infrastructure;

public class QuestionOptionRepositoryTests : IDisposable
{
    private readonly ContentDbContext _context;
    private readonly QuestionOptionRepository _repository;
    private readonly string _dbName;

    public QuestionOptionRepositoryTests()
    {
        _dbName = $"TestDb_{Guid.NewGuid()}.db";
        var options = new DbContextOptionsBuilder<ContentDbContext>()
            .UseSqlite($"Data Source={_dbName}")
            .Options;

        _context = new ContentDbContext(options);
        _context.Database.EnsureCreated(); // Create the database schema
        _repository = new QuestionOptionRepository(_context);
    }

    [Fact]
    public async Task AddAsync_WithValidQuestionOption_ShouldAddQuestionOption()
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

        var questionOption = new QuestionOption("4", question.Id, true);

        // Act
        var result = await _repository.AddAsync(questionOption);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("4", result.OptionText);
        Assert.Equal(question.Id, result.QuestionId);
        Assert.True(result.IsCorrect);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnQuestionOption()
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

        var questionOption = new QuestionOption("4", question.Id, true);
        await _context.QuestionOptions.AddAsync(questionOption);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(questionOption.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(questionOption.Id, result.Id);
        Assert.Equal("4", result.OptionText);
        Assert.True(result.IsCorrect);
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
    public async Task GetAllAsync_WithQuestionOptions_ShouldReturnAllQuestionOptions()
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

        var option1 = new QuestionOption("4", question.Id, true);
        var option2 = new QuestionOption("5", question.Id, false);
        await _context.QuestionOptions.AddRangeAsync(option1, option2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, o => o.OptionText == "4" && o.IsCorrect);
        Assert.Contains(result, o => o.OptionText == "5" && !o.IsCorrect);
    }

    [Fact]
    public async Task GetByQuestionIdAsync_WithExistingQuestionId_ShouldReturnQuestionOptions()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();

        var topic = new Topic("Algebra", subject.Id);
        await _context.Topics.AddAsync(topic);
        await _context.SaveChangesAsync();

        var question1 = new Question("What is 2 + 2?", topic.Id, 1, 5, QuestionSource.Manual);
        var question2 = new Question("What is 3 + 3?", topic.Id, 1, 5, QuestionSource.Manual);
        await _context.Questions.AddRangeAsync(question1, question2);
        await _context.SaveChangesAsync();

        var option1 = new QuestionOption("4", question1.Id, true);
        var option2 = new QuestionOption("5", question1.Id, false);
        var option3 = new QuestionOption("6", question2.Id, true);
        await _context.QuestionOptions.AddRangeAsync(option1, option2, option3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByQuestionIdAsync(question1.Id);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, o => Assert.Equal(question1.Id, o.QuestionId));
        Assert.Contains(result, o => o.OptionText == "4" && o.IsCorrect);
        Assert.Contains(result, o => o.OptionText == "5" && !o.IsCorrect);
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

        var questionOption = new QuestionOption("4", question.Id, true);
        await _context.QuestionOptions.AddAsync(questionOption);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(questionOption.Id);

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
    public async Task UpdateAsync_WithValidQuestionOption_ShouldUpdateQuestionOption()
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

        var questionOption = new QuestionOption("4", question.Id, true);
        await _context.QuestionOptions.AddAsync(questionOption);
        await _context.SaveChangesAsync();

        // Act
        questionOption.UpdateOption("5", false);
        var result = await _repository.UpdateAsync(questionOption);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("5", result.OptionText);
        Assert.False(result.IsCorrect);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldDeleteQuestionOption()
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

        var questionOption = new QuestionOption("4", question.Id, true);
        await _context.QuestionOptions.AddAsync(questionOption);
        await _context.SaveChangesAsync();

        var optionId = questionOption.Id;

        // Act
        await _repository.DeleteAsync(optionId);
        await _context.SaveChangesAsync();

        // Assert
        var deletedOption = await _context.QuestionOptions.FindAsync(optionId);
        Assert.Null(deletedOption);
    }

    [Fact]
    public async Task GetByQuestionIdAsync_WithNoOptions_ShouldReturnEmptyList()
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
        var result = await _repository.GetByQuestionIdAsync(question.Id);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByQuestionIdAsync_WithNonExistingQuestionId_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetByQuestionIdAsync(999);

        // Assert
        Assert.Empty(result);
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
