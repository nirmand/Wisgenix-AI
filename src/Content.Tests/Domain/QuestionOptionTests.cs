using Content.Domain.Entities;
using Content.Domain.Events;
using Wisgenix.SharedKernel.Exceptions;
using Xunit;

namespace Content.Tests.Domain;

public class QuestionOptionTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateQuestionOption()
    {
        // Arrange
        var optionText = "This is a correct answer";
        var questionId = 1;
        var isCorrect = true;

        // Act
        var option = new QuestionOption(optionText, questionId, isCorrect);

        // Assert
        Assert.Equal(optionText, option.OptionText);
        Assert.Equal(questionId, option.QuestionId);
        Assert.Equal(isCorrect, option.IsCorrect);
        Assert.Single(option.DomainEvents);
        Assert.IsType<QuestionOptionCreatedEvent>(option.DomainEvents.First());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidOptionText_ShouldThrowException(string optionText)
    {
        // Act & Assert
        Assert.Throws<DomainValidationException>(() => new QuestionOption(optionText, 1, true));
    }

    [Fact]
    public void Constructor_WithTooLongOptionText_ShouldThrowException()
    {
        // Arrange
        var optionText = new string('a', 4001);

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => new QuestionOption(optionText, 1, true));
    }

    [Fact]
    public void Constructor_WithCorrectAnswer_ShouldSetIsCorrectTrue()
    {
        // Arrange
        var optionText = "Correct answer";
        var questionId = 1;

        // Act
        var option = new QuestionOption(optionText, questionId, true);

        // Assert
        Assert.True(option.IsCorrect);
    }

    [Fact]
    public void Constructor_WithIncorrectAnswer_ShouldSetIsCorrectFalse()
    {
        // Arrange
        var optionText = "Incorrect answer";
        var questionId = 1;

        // Act
        var option = new QuestionOption(optionText, questionId, false);

        // Assert
        Assert.False(option.IsCorrect);
    }

    [Fact]
    public void UpdateOption_WithValidData_ShouldUpdateOption()
    {
        // Arrange
        var option = new QuestionOption("Original text", 1, false);
        var newText = "Updated text";
        var newIsCorrect = true;
        option.ClearDomainEvents();

        // Act
        option.UpdateOption(newText, newIsCorrect);

        // Assert
        Assert.Equal(newText, option.OptionText);
        Assert.Equal(newIsCorrect, option.IsCorrect);
        Assert.Single(option.DomainEvents);
        Assert.IsType<QuestionOptionUpdatedEvent>(option.DomainEvents.First());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateOption_WithInvalidOptionText_ShouldThrowException(string optionText)
    {
        // Arrange
        var option = new QuestionOption("Original text", 1, false);

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => option.UpdateOption(optionText, true));
    }

    [Fact]
    public void UpdateOption_WithTooLongOptionText_ShouldThrowException()
    {
        // Arrange
        var option = new QuestionOption("Original text", 1, false);
        var newText = new string('a', 4001);

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => option.UpdateOption(newText, true));
    }

    [Fact]
    public void UpdateOption_ChangingFromIncorrectToCorrect_ShouldUpdateIsCorrect()
    {
        // Arrange
        var option = new QuestionOption("Answer text", 1, false);
        option.ClearDomainEvents();

        // Act
        option.UpdateOption("Answer text", true);

        // Assert
        Assert.True(option.IsCorrect);
        Assert.Single(option.DomainEvents);
        Assert.IsType<QuestionOptionUpdatedEvent>(option.DomainEvents.First());
    }

    [Fact]
    public void UpdateOption_ChangingFromCorrectToIncorrect_ShouldUpdateIsCorrect()
    {
        // Arrange
        var option = new QuestionOption("Answer text", 1, true);
        option.ClearDomainEvents();

        // Act
        option.UpdateOption("Answer text", false);

        // Assert
        Assert.False(option.IsCorrect);
        Assert.Single(option.DomainEvents);
        Assert.IsType<QuestionOptionUpdatedEvent>(option.DomainEvents.First());
    }

    [Fact]
    public void ClearDomainEvents_ShouldClearAllEvents()
    {
        // Arrange
        var option = new QuestionOption("Answer text", 1, true);

        // Act
        option.ClearDomainEvents();

        // Assert
        Assert.Empty(option.DomainEvents);
    }

    [Fact]
    public void Constructor_WithLongValidText_ShouldCreateOption()
    {
        // Arrange
        var optionText = new string('a', 4000); // Maximum allowed length
        var questionId = 1;

        // Act
        var option = new QuestionOption(optionText, questionId, true);

        // Assert
        Assert.Equal(optionText, option.OptionText);
        Assert.Equal(questionId, option.QuestionId);
        Assert.True(option.IsCorrect);
    }

    [Fact]
    public void UpdateOption_WithSameValues_ShouldStillTriggerEvent()
    {
        // Arrange
        var option = new QuestionOption("Same text", 1, true);
        option.ClearDomainEvents();

        // Act
        option.UpdateOption("Same text", true);

        // Assert
        Assert.Equal("Same text", option.OptionText);
        Assert.True(option.IsCorrect);
        Assert.Single(option.DomainEvents);
        Assert.IsType<QuestionOptionUpdatedEvent>(option.DomainEvents.First());
    }
}
