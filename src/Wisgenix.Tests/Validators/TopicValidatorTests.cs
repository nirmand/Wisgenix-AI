using Wisgenix.DTO;
using Wisgenix.DTO.Validators;
using Xunit;

namespace Wisgenix.Tests.Validators;

public class TopicValidatorTests
{
    private readonly CreateTopicDtoValidator _createValidator;
    private readonly UpdateTopicDtoValidator _updateValidator;

    public TopicValidatorTests()
    {
        _createValidator = new CreateTopicDtoValidator();
        _updateValidator = new UpdateTopicDtoValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CreateValidator_WithInvalidName_ShouldFail(string topicName)
    {
        // Arrange
        var dto = new CreateTopicDto { TopicName = topicName, SubjectID = 1 };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "TopicName" && 
            error.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task CreateValidator_WithTooLongName_ShouldFail()
    {
        // Arrange
        var dto = new CreateTopicDto 
        { 
            TopicName = new string('a', 201),
            SubjectID = 1
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "TopicName" && 
            error.ErrorMessage.Contains("exceed 200 characters"));
    }

    [Theory]
    [InlineData("Topic@Name")]
    [InlineData("Topic#123")]
    [InlineData("Topic-Name$")]
    public async Task CreateValidator_WithInvalidCharacters_ShouldFail(string topicName)
    {
        // Arrange
        var dto = new CreateTopicDto { TopicName = topicName, SubjectID = 1 };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "TopicName" && 
            error.ErrorMessage.Contains("invalid characters"));
    }

    [Fact]
    public async Task CreateValidator_WithInvalidSubjectId_ShouldFail()
    {
        // Arrange
        var dto = new CreateTopicDto { TopicName = "Valid Name", SubjectID = 0 };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "SubjectID" && 
            error.ErrorMessage.Contains("must be greater than 0"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task UpdateValidator_WithInvalidName_ShouldFail(string topicName)
    {
        // Arrange
        var dto = new UpdateTopicDto { TopicName = topicName };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

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
        var dto = new UpdateTopicDto { TopicName = new string('a', 201) };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "TopicName" && 
            error.ErrorMessage.Contains("exceed 200 characters"));
    }

    [Theory]
    [InlineData("Topic@Name")]
    [InlineData("Topic#123")]
    [InlineData("Topic-Name$")]
    public async Task UpdateValidator_WithInvalidCharacters_ShouldFail(string topicName)
    {
        // Arrange
        var dto = new UpdateTopicDto { TopicName = topicName };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "TopicName" && 
            error.ErrorMessage.Contains("invalid characters"));
    }
}