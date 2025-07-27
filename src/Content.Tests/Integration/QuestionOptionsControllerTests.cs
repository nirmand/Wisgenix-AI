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

public class QuestionOptionsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _dbName;

    public QuestionOptionsControllerTests(WebApplicationFactory<Program> factory)
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

        var question = new Question("What is 2 + 2?", topic.Id, 1, 5, QuestionSource.Manual);
        context.Questions.Add(question);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetQuestionOptions_ShouldReturnOkWithQuestionOptions()
    {
        // Act
        var response = await _client.GetAsync("/api/content/question-options");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var optionsResponse = JsonSerializer.Deserialize<JsonElement>(content);
        Assert.True(optionsResponse.TryGetProperty("options", out var optionsArray));
        Assert.True(optionsArray.ValueKind == JsonValueKind.Array);
    }

    [Fact]
    public async Task CreateQuestionOption_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
        var question = await context.Questions.FirstAsync();

        var request = new AddQuestionOptionRequest
        {
            OptionText = "4",
            QuestionId = question.Id,
            IsCorrect = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/content/question-options", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var option = JsonSerializer.Deserialize<JsonElement>(content);
        Assert.Equal("4", option.GetProperty("optionText").GetString());
        Assert.Equal(question.Id, option.GetProperty("questionId").GetInt32());
        Assert.True(option.GetProperty("isCorrect").GetBoolean());
    }

    [Fact]
    public async Task CreateQuestionOption_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new AddQuestionOptionRequest
        {
            OptionText = "", // Invalid empty text
            QuestionId = 1,
            IsCorrect = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/content/question-options", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetQuestionOptionById_WithExistingId_ShouldReturnOkWithQuestionOption()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
        var question = await context.Questions.FirstAsync();
        
        var option = new QuestionOption("5", question.Id, false);
        context.QuestionOptions.Add(option);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/content/question-options/{option.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var optionResponse = JsonSerializer.Deserialize<JsonElement>(content);
        Assert.Equal("5", optionResponse.GetProperty("optionText").GetString());
        Assert.Equal(option.Id, optionResponse.GetProperty("id").GetInt32());
        Assert.False(optionResponse.GetProperty("isCorrect").GetBoolean());
    }

    [Fact]
    public async Task GetQuestionOptionById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/content/question-options/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateQuestionOption_WithValidData_ShouldReturnOkWithUpdatedQuestionOption()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
        var question = await context.Questions.FirstAsync();
        
        var option = new QuestionOption("6", question.Id, false);
        context.QuestionOptions.Add(option);
        await context.SaveChangesAsync();

        var request = new UpdateQuestionOptionRequest
        {
            OptionText = "7",
            QuestionId = question.Id,
            IsCorrect = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/content/question-options/{option.Id}", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var updatedOption = JsonSerializer.Deserialize<JsonElement>(content);
        Assert.Equal("7", updatedOption.GetProperty("optionText").GetString());
        Assert.True(updatedOption.GetProperty("isCorrect").GetBoolean());
    }

    [Fact]
    public async Task UpdateQuestionOption_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
        var question = await context.Questions.FirstAsync();

        var request = new UpdateQuestionOptionRequest
        {
            OptionText = "8",
            QuestionId = question.Id,
            IsCorrect = true
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/content/question-options/999", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteQuestionOption_WithExistingId_ShouldReturnNoContent()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
        var question = await context.Questions.FirstAsync();
        
        var option = new QuestionOption("9", question.Id, false);
        context.QuestionOptions.Add(option);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/content/question-options/{option.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/content/question-options/{option.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteQuestionOption_WithNonExistingId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/content/question-options/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetQuestionOptionsByQuestionId_WithExistingQuestionId_ShouldReturnOkWithQuestionOptions()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
        var question = await context.Questions.FirstAsync();
        
        var option1 = new QuestionOption("10", question.Id, true);
        var option2 = new QuestionOption("11", question.Id, false);
        context.QuestionOptions.AddRange(option1, option2);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/content/question-options/by-question/{question.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var optionsResponse = JsonSerializer.Deserialize<JsonElement>(content);
        Assert.True(optionsResponse.TryGetProperty("options", out var optionsArray));
        Assert.True(optionsArray.ValueKind == JsonValueKind.Array);
        Assert.True(optionsArray.GetArrayLength() >= 2);
    }

    [Fact]
    public async Task GetQuestionOptionsByQuestionId_WithNonExistingQuestionId_ShouldReturnOkWithEmptyArray()
    {
        // Act
        var response = await _client.GetAsync("/api/content/question-options/by-question/999");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var optionsResponse = JsonSerializer.Deserialize<JsonElement>(content);
        Assert.True(optionsResponse.TryGetProperty("options", out var optionsArray));
        Assert.True(optionsArray.ValueKind == JsonValueKind.Array);
        Assert.Equal(0, optionsArray.GetArrayLength());
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
