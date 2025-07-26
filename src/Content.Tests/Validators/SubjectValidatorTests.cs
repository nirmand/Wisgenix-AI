using Content.Application.DTOs;
using Content.Application.Validators;
using Xunit;

namespace Content.Tests.Validators;

public class SubjectValidatorTests
{
    private readonly AddSubjectRequestValidator _addValidator;
    private readonly UpdateSubjectRequestValidator _updateValidator;

    public SubjectValidatorTests()
    {
        _addValidator = new AddSubjectRequestValidator();
        _updateValidator = new UpdateSubjectRequestValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task AddValidator_WithInvalidName_ShouldFail(string subjectName)
    {
        // Arrange
        var request = new AddSubjectRequest { SubjectName = subjectName };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "SubjectName" && 
            error.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task AddValidator_WithTooLongName_ShouldFail()
    {
        // Arrange
        var request = new AddSubjectRequest { SubjectName = new string('a', 201) };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "SubjectName" && 
            error.ErrorMessage.Contains("exceed 200 characters"));
    }

    [Theory]
    [InlineData("Subject>Name")]
    [InlineData("Subject<123")]
    [InlineData("Subject&Name")]
    [InlineData("Subject\"Name")]
    [InlineData("Subject'Name")]
    public async Task AddValidator_WithInvalidCharacters_ShouldFail(string subjectName)
    {
        // Arrange
        var request = new AddSubjectRequest { SubjectName = subjectName };

        // Act
        var result = await _addValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "SubjectName" && 
            error.ErrorMessage.Contains("invalid characters"));
    }

    [Fact]
    public async Task AddValidator_WithValidName_ShouldPass()
    {
        // Arrange
        var request = new AddSubjectRequest { SubjectName = "Valid Subject Name" };

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
    public async Task UpdateValidator_WithInvalidName_ShouldFail(string subjectName)
    {
        // Arrange
        var request = new UpdateSubjectRequest { SubjectName = subjectName };

        // Act
        var result = await _updateValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "SubjectName" && 
            error.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task UpdateValidator_WithTooLongName_ShouldFail()
    {
        // Arrange
        var request = new UpdateSubjectRequest { SubjectName = new string('a', 201) };

        // Act
        var result = await _updateValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "SubjectName" && 
            error.ErrorMessage.Contains("exceed 200 characters"));
    }

    [Theory]
    [InlineData("<SubjectName")]
    [InlineData("Subject>123")]
    [InlineData("Subject&Name")]
    [InlineData("Subject\"Name")]
    [InlineData("Subject'Name")]
    public async Task UpdateValidator_WithInvalidCharacters_ShouldFail(string subjectName)
    {
        // Arrange
        var request = new UpdateSubjectRequest { SubjectName = subjectName };

        // Act
        var result = await _updateValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "SubjectName" && 
            error.ErrorMessage.Contains("invalid characters"));
    }

    [Fact]
    public async Task UpdateValidator_WithValidName_ShouldPass()
    {
        // Arrange
        var request = new UpdateSubjectRequest { SubjectName = "Valid Subject Name" };

        // Act
        var result = await _updateValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
