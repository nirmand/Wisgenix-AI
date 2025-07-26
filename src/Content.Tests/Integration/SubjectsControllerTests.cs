using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using Content.Application.DTOs;
using Content.Infrastructure.Data;
using Xunit;

namespace Content.Tests.Integration;

public class SubjectsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SubjectsControllerTests(WebApplicationFactory<Program> factory)
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
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetSubjects_ShouldReturnEmptyList_WhenNoSubjects()
    {
        // Act
        var response = await _client.GetAsync("/api/content/subjects");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetSubjectsResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.Empty(result.Subjects);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task CreateSubject_WithValidData_ShouldReturnCreatedSubject()
    {
        // Arrange
        var request = new AddSubjectRequest { SubjectName = "Mathematics" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/content/subjects", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetSubjectResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.Equal("Mathematics", result.SubjectName);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task CreateSubject_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new AddSubjectRequest { SubjectName = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/content/subjects", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetSubject_WithExistingId_ShouldReturnSubject()
    {
        // Arrange
        var createRequest = new AddSubjectRequest { SubjectName = "Physics" };
        var createResponse = await _client.PostAsJsonAsync("/api/content/subjects", createRequest);
        var createdSubject = JsonSerializer.Deserialize<GetSubjectResponse>(
            await createResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Act
        var response = await _client.GetAsync($"/api/content/subjects/{createdSubject!.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetSubjectResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.Equal("Physics", result.SubjectName);
        Assert.Equal(createdSubject.Id, result.Id);
    }

    [Fact]
    public async Task GetSubject_WithNonExistingId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/content/subjects/999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSubject_WithValidData_ShouldReturnUpdatedSubject()
    {
        // Arrange
        var createRequest = new AddSubjectRequest { SubjectName = "Chemistry" };
        var createResponse = await _client.PostAsJsonAsync("/api/content/subjects", createRequest);
        var createdSubject = JsonSerializer.Deserialize<GetSubjectResponse>(
            await createResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var updateRequest = new UpdateSubjectRequest { SubjectName = "Advanced Chemistry" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/content/subjects/{createdSubject!.Id}", updateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetSubjectResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.Equal("Advanced Chemistry", result.SubjectName);
        Assert.Equal(createdSubject.Id, result.Id);
    }

    [Fact]
    public async Task DeleteSubject_WithExistingId_ShouldReturnNoContent()
    {
        // Arrange
        var createRequest = new AddSubjectRequest { SubjectName = "Biology" };
        var createResponse = await _client.PostAsJsonAsync("/api/content/subjects", createRequest);
        var createdSubject = JsonSerializer.Deserialize<GetSubjectResponse>(
            await createResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Act
        var response = await _client.DeleteAsync($"/api/content/subjects/{createdSubject!.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify subject is deleted
        var getResponse = await _client.GetAsync($"/api/content/subjects/{createdSubject.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
