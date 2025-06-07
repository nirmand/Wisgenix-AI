using Wisgenix.Common;
using Wisgenix.DTO;
using Wisgenix.DTO.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Wisgenix.Tests.Validators;

public class QuestionDtoValidatorTests
{
    private readonly QuestionDtoWriteValidator _baseValidator;
    private readonly CreateQuestionDtoValidator _createValidator;
    private readonly UpdateQuestionDtoValidator _updateValidator;

    public QuestionDtoValidatorTests()
    {
        _baseValidator = new QuestionDtoWriteValidator();
        _createValidator = new CreateQuestionDtoValidator();
        _updateValidator = new UpdateQuestionDtoValidator();
    }

    [Fact]
    public void QuestionText_WhenEmpty_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateQuestionDto { QuestionText = "" };

        // Act & Assert
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.QuestionText)
            .WithErrorMessage("Question text is required");
    }

    [Fact]
    public void QuestionText_WhenTooLong_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateQuestionDto { QuestionText = new string('a', 1001) };

        // Act & Assert
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.QuestionText)
            .WithErrorMessage("Question text cannot exceed 1000 characters");
    }

    [Fact]
    public void TopicId_WhenZeroOrNegative_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateQuestionDto { TopicID = 0 };

        // Act & Assert
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.TopicID)
            .WithErrorMessage("Topic ID must be greater than 0");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void DifficultyLevel_WhenOutOfRange_ShouldHaveError(int level)
    {
        // Arrange
        var dto = new CreateQuestionDto { DifficultyLevel = level };

        // Act & Assert
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DifficultyLevel)
            .WithErrorMessage("Difficulty level must be between 1 and 5");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void MaxScore_WhenOutOfRange_ShouldHaveError(int score)
    {
        // Arrange
        var dto = new CreateQuestionDto { MaxScore = score };

        // Act & Assert
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.MaxScore)
            .WithErrorMessage("Max score must be between 1 and 10");
    }

    [Fact]
    public void QuestionSourceReference_WhenInvalidUrl_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateQuestionDto { QuestionSourceReference = "not-a-url" };

        // Act & Assert
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.QuestionSourceReference)
            .WithErrorMessage("Question source reference must be a valid URL");
    }

    [Fact]
    public void ValidDto_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new CreateQuestionDto
        {
            QuestionText = "Valid question text",
            TopicID = 1,
            DifficultyLevel = 3,
            MaxScore = 5,
            GeneratedBy = QuestionSource.AI,
            QuestionSourceReference = "https://valid-url.com"
        };

        // Act & Assert
        var result = _createValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateValidator_ShouldUseBaseValidatorRules()
    {
        // Arrange
        var dto = new UpdateQuestionDto
        {
            QuestionText = "",
            TopicID = 0
        };

        // Act & Assert
        var result = _updateValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.QuestionText);
        result.ShouldHaveValidationErrorFor(x => x.TopicID);
    }
}