using Content.Application.DTOs;
using Content.Application.Validators;
using Wisgenix.SharedKernel.Domain.Enums;
using Xunit;

namespace Content.Tests.Validators;

public class QuestionValidatorTests
{
    private readonly AddQuestionRequestValidator _addValidator;
    private readonly UpdateQuestionRequestValidator _updateValidator;

    public QuestionValidatorTests()
    {
        _addValidator = new AddQuestionRequestValidator();
        _updateValidator = new UpdateQuestionRequestValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task AddValidator_WithInvalidQuestionText_ShouldFail(string questionText)
    {
        // Arrange
        var request = new AddQuestionRequest 
        { 
            QuestionText = questionText,
            TopicId = 1,
            DifficultyLevel = 3,
            MaxScore = 5,
            GeneratedBy = QuestionSource.Manual
        };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "QuestionText" && 
            error.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task AddValidator_WithTooLongQuestionText_ShouldFail()
    {
        // Arrange
        var request = new AddQuestionRequest 
        { 
            QuestionText = new string('a', 1001),
            TopicId = 1,
            DifficultyLevel = 3,
            MaxScore = 5,
            GeneratedBy = QuestionSource.Manual
        };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "QuestionText" && 
            error.ErrorMessage.Contains("exceed 1000 characters"));
    }

    [Fact]
    public async Task AddValidator_WithInvalidTopicId_ShouldFail()
    {
        // Arrange
        var request = new AddQuestionRequest 
        { 
            QuestionText = "Valid question text?",
            TopicId = 0,
            DifficultyLevel = 3,
            MaxScore = 5,
            GeneratedBy = QuestionSource.Manual
        };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "TopicId" && 
            error.ErrorMessage.Contains("must be greater than 0"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public async Task AddValidator_WithInvalidDifficultyLevel_ShouldFail(int difficultyLevel)
    {
        // Arrange
        var request = new AddQuestionRequest 
        { 
            QuestionText = "Valid question text?",
            TopicId = 1,
            DifficultyLevel = difficultyLevel,
            MaxScore = 5,
            GeneratedBy = QuestionSource.Manual
        };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "DifficultyLevel" && 
            error.ErrorMessage.Contains("between 1 and 5"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public async Task AddValidator_WithInvalidMaxScore_ShouldFail(int maxScore)
    {
        // Arrange
        var request = new AddQuestionRequest 
        { 
            QuestionText = "Valid question text?",
            TopicId = 1,
            DifficultyLevel = 3,
            MaxScore = maxScore,
            GeneratedBy = QuestionSource.Manual
        };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "MaxScore" && 
            error.ErrorMessage.Contains("between 1 and 10"));
    }

    [Fact]
    public async Task AddValidator_WithInvalidUrl_ShouldFail()
    {
        // Arrange
        var request = new AddQuestionRequest 
        { 
            QuestionText = "Valid question text?",
            TopicId = 1,
            DifficultyLevel = 3,
            MaxScore = 5,
            GeneratedBy = QuestionSource.Manual,
            QuestionSourceReference = "not-a-valid-url"
        };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "QuestionSourceReference" && 
            error.ErrorMessage.Contains("valid URL"));
    }

    [Fact]
    public async Task AddValidator_WithValidData_ShouldPass()
    {
        // Arrange
        var request = new AddQuestionRequest 
        { 
            QuestionText = "What is the capital of France?",
            TopicId = 1,
            DifficultyLevel = 2,
            MaxScore = 3,
            GeneratedBy = QuestionSource.Manual,
            QuestionSourceReference = "https://example.com/source"
        };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task AddValidator_WithNullUrl_ShouldPass()
    {
        // Arrange
        var request = new AddQuestionRequest 
        { 
            QuestionText = "What is the capital of France?",
            TopicId = 1,
            DifficultyLevel = 2,
            MaxScore = 3,
            GeneratedBy = QuestionSource.Manual,
            QuestionSourceReference = null
        };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task UpdateValidator_WithValidData_ShouldPass()
    {
        // Arrange
        var request = new UpdateQuestionRequest 
        { 
            QuestionText = "What is the capital of Germany?",
            TopicId = 1,
            DifficultyLevel = 3,
            MaxScore = 4,
            GeneratedBy = QuestionSource.AI,
            QuestionSourceReference = "https://example.com/updated-source"
        };

        // Act
        var result = await _updateValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
