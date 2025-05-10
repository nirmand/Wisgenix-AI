using AIUpskillingPlatform.DTO;
using AIUpskillingPlatform.DTO.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace AIUpskillingPlatform.Tests.Validators;

public class QuestionOptionDtoValidatorTests
{
    private readonly QuestionOptionDtoWriteValidator _baseValidator;
    private readonly CreateQuestionOptionDtoValidator _createValidator;
    private readonly UpdateQuestionOptionDtoValidator _updateValidator;

    public QuestionOptionDtoValidatorTests()
    {
        _baseValidator = new QuestionOptionDtoWriteValidator();
        _createValidator = new CreateQuestionOptionDtoValidator();
        _updateValidator = new UpdateQuestionOptionDtoValidator();
    }

    [Fact]
    public void OptionText_WhenEmpty_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateQuestionOptionDto { OptionText = "" };

        // Act & Assert
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.OptionText)
            .WithErrorMessage("Option text is required");
    }

    [Fact]
    public void OptionText_WhenTooLong_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateQuestionOptionDto { OptionText = new string('a', 4001) };

        // Act & Assert
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.OptionText)
            .WithErrorMessage("Option text cannot exceed 4000 characters");
    }

    [Fact]
    public void QuestionId_WhenZeroOrNegative_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateQuestionOptionDto { QuestionID = 0 };

        // Act & Assert
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.QuestionID)
            .WithErrorMessage("Question ID must be greater than 0");
    }

    [Fact]
    public void ValidDto_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new CreateQuestionOptionDto
        {
            OptionText = "Valid option text",
            QuestionID = 1,
            IsCorrect = true
        };

        // Act & Assert
        var result = _createValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateValidator_ShouldUseBaseValidatorRules()
    {
        // Arrange
        var dto = new UpdateQuestionOptionDto
        {
            OptionText = "",
            QuestionID = 0
        };

        // Act & Assert
        var result = _updateValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.OptionText);
        result.ShouldHaveValidationErrorFor(x => x.QuestionID);
    }
}