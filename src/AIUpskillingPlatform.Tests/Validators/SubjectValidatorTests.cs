using AIUpskillingPlatform.DTO;
using AIUpskillingPlatform.DTO.Validators;
using Xunit;

namespace AIUpskillingPlatform.Tests.Validators;

public class SubjectValidatorTests
{
    private readonly CreateSubjectDtoValidator _createValidator;
    private readonly UpdateSubjectDtoValidator _updateValidator;

    public SubjectValidatorTests()
    {
        _createValidator = new CreateSubjectDtoValidator();
        _updateValidator = new UpdateSubjectDtoValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CreateValidator_WithInvalidName_ShouldFail(string subjectName)
    {
        // Arrange
        var dto = new CreateSubjectDto { SubjectName = subjectName };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "SubjectName" && 
            error.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task CreateValidator_WithTooLongName_ShouldFail()
    {
        // Arrange
        var dto = new CreateSubjectDto { SubjectName = new string('a', 201) };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "SubjectName" && 
            error.ErrorMessage.Contains("exceed 200 characters"));
    }

    [Theory]
    [InlineData("Subject@Name")]
    [InlineData("Subject#123")]
    [InlineData("Subject-Name")]
    public async Task CreateValidator_WithInvalidCharacters_ShouldFail(string subjectName)
    {
        // Arrange
        var dto = new CreateSubjectDto { SubjectName = subjectName };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "SubjectName" && 
            error.ErrorMessage.Contains("invalid characters"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task UpdateValidator_WithInvalidName_ShouldFail(string subjectName)
    {
        // Arrange
        var dto = new UpdateSubjectDto { SubjectName = subjectName };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

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
        var dto = new UpdateSubjectDto { SubjectName = new string('a', 201) };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "SubjectName" && 
            error.ErrorMessage.Contains("exceed 200 characters"));
    }

    [Theory]
    [InlineData("Subject@Name")]
    [InlineData("Subject#123")]
    [InlineData("Subject-Name")]
    public async Task UpdateValidator_WithInvalidCharacters_ShouldFail(string subjectName)
    {
        // Arrange
        var dto = new UpdateSubjectDto { SubjectName = subjectName };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.PropertyName == "SubjectName" && 
            error.ErrorMessage.Contains("invalid characters"));
    }
}