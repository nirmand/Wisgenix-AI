using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using Content.Application.DTOs;
using Content.Infrastructure.Data;
using Xunit;

namespace Content.Tests.Integration;

public class TopicsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TopicsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ContentDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<ContentDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetTopics_ShouldReturnEmptyList_WhenNoTopics()
    {
        // Act
        var response = await _client.GetAsync("/api/content/topics");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetTopicsResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.Empty(result.Topics);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task CreateTopic_WithValidData_ShouldReturnCreatedTopic()
    {
        // Arrange - First create a subject
        var subjectRequest = new AddSubjectRequest { SubjectName = "Mathematics" };
        var subjectResponse = await _client.PostAsJsonAsync("/api/content/subjects", subjectRequest);
        var createdSubject = JsonSerializer.Deserialize<GetSubjectResponse>(
            await subjectResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var topicRequest = new AddTopicRequest 
        { 
            TopicName = "Algebra", 
            SubjectId = createdSubject!.Id 
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/content/topics", topicRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetTopicResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.Equal("Algebra", result.TopicName);
        Assert.Equal(createdSubject.Id, result.SubjectId);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task CreateTopic_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new AddTopicRequest { TopicName = "", SubjectId = 1 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/content/topics", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetTopic_WithExistingId_ShouldReturnTopic()
    {
        // Arrange - Create subject and topic
        var subjectRequest = new AddSubjectRequest { SubjectName = "Physics" };
        var subjectResponse = await _client.PostAsJsonAsync("/api/content/subjects", subjectRequest);
        var createdSubject = JsonSerializer.Deserialize<GetSubjectResponse>(
            await subjectResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var topicRequest = new AddTopicRequest 
        { 
            TopicName = "Mechanics", 
            SubjectId = createdSubject!.Id 
        };
        var topicResponse = await _client.PostAsJsonAsync("/api/content/topics", topicRequest);
        var createdTopic = JsonSerializer.Deserialize<GetTopicResponse>(
            await topicResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Act
        var response = await _client.GetAsync($"/api/content/topics/{createdTopic!.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetTopicResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.Equal("Mechanics", result.TopicName);
        Assert.Equal(createdTopic.Id, result.Id);
    }

    [Fact]
    public async Task GetTopic_WithNonExistingId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/content/topics/999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTopicsBySubject_WithExistingSubjectId_ShouldReturnTopics()
    {
        // Arrange - Create subject and topics
        var subjectRequest = new AddSubjectRequest { SubjectName = "Chemistry" };
        var subjectResponse = await _client.PostAsJsonAsync("/api/content/subjects", subjectRequest);
        var createdSubject = JsonSerializer.Deserialize<GetSubjectResponse>(
            await subjectResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var topic1Request = new AddTopicRequest 
        { 
            TopicName = "Organic Chemistry", 
            SubjectId = createdSubject!.Id 
        };
        var topic2Request = new AddTopicRequest 
        { 
            TopicName = "Inorganic Chemistry", 
            SubjectId = createdSubject.Id 
        };

        await _client.PostAsJsonAsync("/api/content/topics", topic1Request);
        await _client.PostAsJsonAsync("/api/content/topics", topic2Request);

        // Act
        var response = await _client.GetAsync($"/api/content/topics/by-subject/{createdSubject.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetTopicsResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Contains(result.Topics, t => t.TopicName == "Organic Chemistry");
        Assert.Contains(result.Topics, t => t.TopicName == "Inorganic Chemistry");
    }

    [Fact]
    public async Task UpdateTopic_WithValidData_ShouldReturnUpdatedTopic()
    {
        // Arrange - Create subject and topic
        var subjectRequest = new AddSubjectRequest { SubjectName = "Biology" };
        var subjectResponse = await _client.PostAsJsonAsync("/api/content/subjects", subjectRequest);
        var createdSubject = JsonSerializer.Deserialize<GetSubjectResponse>(
            await subjectResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var topicRequest = new AddTopicRequest 
        { 
            TopicName = "Genetics", 
            SubjectId = createdSubject!.Id 
        };
        var topicResponse = await _client.PostAsJsonAsync("/api/content/topics", topicRequest);
        var createdTopic = JsonSerializer.Deserialize<GetTopicResponse>(
            await topicResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var updateRequest = new UpdateTopicRequest 
        { 
            TopicName = "Advanced Genetics", 
            SubjectId = createdSubject.Id 
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/content/topics/{createdTopic!.Id}", updateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetTopicResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.Equal("Advanced Genetics", result.TopicName);
        Assert.Equal(createdTopic.Id, result.Id);
    }

    [Fact]
    public async Task DeleteTopic_WithExistingId_ShouldReturnNoContent()
    {
        // Arrange - Create subject and topic
        var subjectRequest = new AddSubjectRequest { SubjectName = "History" };
        var subjectResponse = await _client.PostAsJsonAsync("/api/content/subjects", subjectRequest);
        var createdSubject = JsonSerializer.Deserialize<GetSubjectResponse>(
            await subjectResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var topicRequest = new AddTopicRequest 
        { 
            TopicName = "World War II", 
            SubjectId = createdSubject!.Id 
        };
        var topicResponse = await _client.PostAsJsonAsync("/api/content/topics", topicRequest);
        var createdTopic = JsonSerializer.Deserialize<GetTopicResponse>(
            await topicResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Act
        var response = await _client.DeleteAsync($"/api/content/topics/{createdTopic!.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify topic is deleted
        var getResponse = await _client.GetAsync($"/api/content/topics/{createdTopic.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
