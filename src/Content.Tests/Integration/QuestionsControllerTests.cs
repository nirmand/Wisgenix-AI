using Content.Application.DTOs;
using Content.Domain.Entities;
using Content.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Wisgenix.SharedKernel.Domain.Enums;
using Xunit;

namespace Content.Tests.Integration;

public class QuestionsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _dbName;

    public QuestionsControllerTests(WebApplicationFactory<Program> factory)
    {
        _dbName = $"TestDb_{Guid.NewGuid()}.db";
        
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ContentDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add test database
                services.AddDbContext<ContentDbContext>(options =>
                    options.UseSqlite($"Data Source={_dbName}"));
            });
        });

        _client = _factory.CreateClient();
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
        context.Database.EnsureCreated();

        // Seed test data
        var subject = new Subject("Mathematics");
        context.Subjects.Add(subject);
        context.SaveChanges();

        var topic = new Topic("Algebra", subject.Id);
        context.Topics.Add(topic);
        context.SaveChanges();

        // Add sample questions for tests
        var question1 = new Question("What is 2 + 2?", topic.Id, 1, 5, QuestionSource.Manual);
        var question2 = new Question("What is 3 + 3?", topic.Id, 2, 7, QuestionSource.AI);
        context.Questions.AddRange(question1, question2);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetQuestions_ShouldReturnOkWithQuestions()
    {
        // Act
        var response = await _client.GetAsync("/api/content/questions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var questionsResponse = JsonSerializer.Deserialize<JsonElement>(content);
        Assert.True(questionsResponse.TryGetProperty("questions", out var questionsArray));
        Assert.True(questionsArray.ValueKind == JsonValueKind.Array);
        Assert.True(questionsArray.GetArrayLength() > 0);
    }

    [Fact]
    public async Task CreateQuestion_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
        var topic = await context.Topics.FirstAsync();

        var request = new AddQuestionRequest
        {
            QuestionText = "What is 6 + 6?",
            TopicId = topic.Id,
            DifficultyLevel = 1,
            MaxScore = 5,
            GeneratedBy = QuestionSource.Manual,
            QuestionSourceReference = "https://example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/content/questions", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var question = JsonSerializer.Deserialize<JsonElement>(content);
        Assert.Equal("What is 6 + 6?", question.GetProperty("questionText").GetString());
        Assert.Equal(topic.Id, question.GetProperty("topicId").GetInt32());
    }

    [Fact]
    public async Task CreateQuestion_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new AddQuestionRequest
        {
            QuestionText = "", // Invalid empty text
            TopicId = 1,
            DifficultyLevel = 1,
            MaxScore = 5,
            GeneratedBy = QuestionSource.Manual
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/content/questions", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetQuestionById_WithExistingId_ShouldReturnOkWithQuestion()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
        var topic = await context.Topics.FirstAsync();
        
        var question = new Question("What is 7 + 7?", topic.Id, 2, 7, QuestionSource.AI);
        context.Questions.Add(question);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/content/questions/{question.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var questionResponse = JsonSerializer.Deserialize<JsonElement>(content);
        Assert.Equal("What is 7 + 7?", questionResponse.GetProperty("questionText").GetString());
        Assert.Equal(question.Id, questionResponse.GetProperty("id").GetInt32());
    }

    [Fact]
    public async Task GetQuestionById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/content/questions/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateQuestion_WithValidData_ShouldReturnOkWithUpdatedQuestion()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
        var topic = await context.Topics.FirstAsync();
        
        var question = new Question("What is 4 + 4?", topic.Id, 1, 5, QuestionSource.Manual);
        context.Questions.Add(question);
        await context.SaveChangesAsync();

        var request = new UpdateQuestionRequest
        {
            QuestionText = "What is 5 + 5?",
            TopicId = topic.Id,
            DifficultyLevel = 2,
            MaxScore = 8,
            GeneratedBy = QuestionSource.AI,
            QuestionSourceReference = "https://updated.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/content/questions/{question.Id}", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var updatedQuestion = JsonSerializer.Deserialize<JsonElement>(content);
        Assert.Equal("What is 5 + 5?", updatedQuestion.GetProperty("questionText").GetString());
        Assert.Equal(2, updatedQuestion.GetProperty("difficultyLevel").GetInt32());
        Assert.Equal(8, updatedQuestion.GetProperty("maxScore").GetInt32());
    }

    [Fact]
    public async Task UpdateQuestion_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
        var topic = await context.Topics.FirstAsync();

        var request = new UpdateQuestionRequest
        {
            QuestionText = "What is 5 + 5?",
            TopicId = topic.Id,
            DifficultyLevel = 2,
            MaxScore = 8,
            GeneratedBy = QuestionSource.AI
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/content/questions/999", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteQuestion_WithExistingId_ShouldReturnNoContent()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
        var topic = await context.Topics.FirstAsync();
        
        var question = new Question("What is 6 + 6?", topic.Id, 1, 5, QuestionSource.Manual);
        context.Questions.Add(question);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/content/questions/{question.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/content/questions/{question.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteQuestion_WithNonExistingId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/content/questions/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetQuestionsByTopicId_WithExistingTopicId_ShouldReturnOkWithQuestions()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
        var topic = await context.Topics.FirstAsync();
        
        var question1 = new Question("What is 7 + 7?", topic.Id, 1, 5, QuestionSource.Manual);
        var question2 = new Question("What is 8 + 8?", topic.Id, 2, 7, QuestionSource.AI);
        context.Questions.AddRange(question1, question2);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/content/questions/by-topic/{topic.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var questionsResponse = JsonSerializer.Deserialize<JsonElement>(content);
        Assert.True(questionsResponse.TryGetProperty("questions", out var questionsArray));
        Assert.True(questionsArray.ValueKind == JsonValueKind.Array);
        Assert.True(questionsArray.GetArrayLength() >= 2);
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
        
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
