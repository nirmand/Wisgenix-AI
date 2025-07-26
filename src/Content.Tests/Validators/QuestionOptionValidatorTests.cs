using Content.Application.DTOs;
using Content.Application.Validators;
using Xunit;

namespace Content.Tests.Validators;

public class QuestionOptionValidatorTests
{
    private readonly AddQuestionOptionRequestValidator _addValidator;
    private readonly UpdateQuestionOptionRequestValidator _updateValidator;

    public QuestionOptionValidatorTests()
    {
        _addValidator = new AddQuestionOptionRequestValidator();
        _updateValidator = new UpdateQuestionOptionRequestValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task AddValidator_WithInvalidOptionText_ShouldFail(string optionText)
    {
        // Arrange
        var request = new AddQuestionOptionRequest 
        { 
            OptionText = optionText,
            QuestionId = 1,
            IsCorrect = true
        };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "OptionText" && 
            error.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task AddValidator_WithTooLongOptionText_ShouldFail()
    {
        // Arrange
        var request = new AddQuestionOptionRequest 
        { 
            OptionText = new string('a', 4001),
            QuestionId = 1,
            IsCorrect = true
        };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "OptionText" && 
            error.ErrorMessage.Contains("exceed 4000 characters"));
    }

    [Fact]
    public async Task AddValidator_WithInvalidQuestionId_ShouldFail()
    {
        // Arrange
        var request = new AddQuestionOptionRequest 
        { 
            OptionText = "Valid option text",
            QuestionId = 0,
            IsCorrect = true
        };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "QuestionId" && 
            error.ErrorMessage.Contains("must be greater than 0"));
    }

    [Fact]
    public async Task AddValidator_WithValidData_ShouldPass()
    {
        // Arrange
        var request = new AddQuestionOptionRequest 
        { 
            OptionText = "This is a valid option",
            QuestionId = 1,
            IsCorrect = true
        };

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
    public async Task UpdateValidator_WithInvalidOptionText_ShouldFail(string optionText)
    {
        // Arrange
        var request = new UpdateQuestionOptionRequest 
        { 
            OptionText = optionText,
            QuestionId = 1,
            IsCorrect = false
        };

        // Act
        var result = await _updateValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "OptionText" && 
            error.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task UpdateValidator_WithTooLongOptionText_ShouldFail()
    {
        // Arrange
        var request = new UpdateQuestionOptionRequest 
        { 
            OptionText = new string('a', 4001),
            QuestionId = 1,
            IsCorrect = false
        };

        // Act
        var result = await _updateValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "OptionText" && 
            error.ErrorMessage.Contains("exceed 4000 characters"));
    }

    [Fact]
    public async Task UpdateValidator_WithInvalidQuestionId_ShouldFail()
    {
        // Arrange
        var request = new UpdateQuestionOptionRequest 
        { 
            OptionText = "Valid option text",
            QuestionId = 0,
            IsCorrect = false
        };

        // Act
        var result = await _updateValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "QuestionId" && 
            error.ErrorMessage.Contains("must be greater than 0"));
    }

    [Fact]
    public async Task UpdateValidator_WithValidData_ShouldPass()
    {
        // Arrange
        var request = new UpdateQuestionOptionRequest 
        { 
            OptionText = "This is an updated valid option",
            QuestionId = 1,
            IsCorrect = false
        };

        // Act
        var result = await _updateValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
