using Content.Application.DTOs;
using Content.Application.Validators;
using Xunit;

namespace Content.Tests.Validators;

public class TopicValidatorTests
{
    private readonly AddTopicRequestValidator _addValidator;
    private readonly UpdateTopicRequestValidator _updateValidator;

    public TopicValidatorTests()
    {
        _addValidator = new AddTopicRequestValidator();
        _updateValidator = new UpdateTopicRequestValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task AddValidator_WithInvalidName_ShouldFail(string topicName)
    {
        // Arrange
        var request = new AddTopicRequest { TopicName = topicName, SubjectId = 1 };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "TopicName" && 
            error.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task AddValidator_WithTooLongName_ShouldFail()
    {
        // Arrange
        var request = new AddTopicRequest 
        { 
            TopicName = new string('a', 201),
            SubjectId = 1
        };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "TopicName" && 
            error.ErrorMessage.Contains("exceed 200 characters"));
    }

    [Theory]
    [InlineData("Topic>Name")]
    [InlineData("<Topic123")]
    [InlineData("Topic&Name")]
    [InlineData("Topic\"Name")]
    [InlineData("Topic'Name")]
    public async Task AddValidator_WithInvalidCharacters_ShouldFail(string topicName)
    {
        // Arrange
        var request = new AddTopicRequest { TopicName = topicName, SubjectId = 1 };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "TopicName" && 
            error.ErrorMessage.Contains("invalid characters"));
    }

    [Fact]
    public async Task AddValidator_WithInvalidSubjectId_ShouldFail()
    {
        // Arrange
        var request = new AddTopicRequest { TopicName = "Valid Name", SubjectId = 0 };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "SubjectId" && 
            error.ErrorMessage.Contains("must be greater than 0"));
    }

    [Fact]
    public async Task AddValidator_WithValidData_ShouldPass()
    {
        // Arrange
        var request = new AddTopicRequest { TopicName = "Valid Topic Name", SubjectId = 1 };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task UpdateValidator_WithInvalidName_ShouldFail(string topicName)
    {
        // Arrange
        var request = new UpdateTopicRequest { TopicName = topicName, SubjectId = 1 };

        // Act
        var result = await _updateValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "TopicName" && 
            error.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task UpdateValidator_WithTooLongName_ShouldFail()
    {
        // Arrange
        var request = new UpdateTopicRequest { TopicName = new string('a', 201), SubjectId = 1 };

        // Act
        var result = await _updateValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "TopicName" && 
            error.ErrorMessage.Contains("exceed 200 characters"));
    }

    [Theory]
    [InlineData("Topic>Name")]
    [InlineData("Topic123<")]
    [InlineData("Topic&Name")]
    [InlineData("Topic\"Name")]
    [InlineData("Topic'Name")]
    public async Task UpdateValidator_WithInvalidCharacters_ShouldFail(string topicName)
    {
        // Arrange
        var request = new UpdateTopicRequest { TopicName = topicName, SubjectId = 1 };

        // Act
        var result = await _updateValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "TopicName" && 
            error.ErrorMessage.Contains("invalid characters"));
    }

    [Fact]
    public async Task UpdateValidator_WithInvalidSubjectId_ShouldFail()
    {
        // Arrange
        var request = new UpdateTopicRequest { TopicName = "Valid Name", SubjectId = 0 };

        // Act
        var result = await _updateValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "SubjectId" && 
            error.ErrorMessage.Contains("must be greater than 0"));
    }

    [Fact]
    public async Task UpdateValidator_WithValidData_ShouldPass()
    {
        // Arrange
        var request = new UpdateTopicRequest { TopicName = "Valid Topic Name", SubjectId = 1 };

        // Act
        var result = await _updateValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
