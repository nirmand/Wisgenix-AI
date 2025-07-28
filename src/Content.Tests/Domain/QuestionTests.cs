using Content.Domain.Entities;
using Content.Domain.Events;
using Wisgenix.SharedKernel.Exceptions;
using Wisgenix.SharedKernel.Domain.Enums;
using Xunit;

namespace Content.Tests.Domain;

public class QuestionTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateQuestion()
    {
        // Arrange
        var questionText = "What is 2 + 2?";
        var topicId = 1;
        var difficultyLevel = 2;
        var maxScore = 5;
        var generatedBy = QuestionSource.Manual;
        var sourceReference = "https://example.com/source";

        // Act
        var question = new Question(questionText, topicId, difficultyLevel, maxScore, generatedBy, sourceReference);

        // Assert
        Assert.Equal(questionText, question.QuestionText);
        Assert.Equal(topicId, question.TopicId);
        Assert.Equal(difficultyLevel, question.DifficultyLevel);
        Assert.Equal(maxScore, question.MaxScore);
        Assert.Equal(generatedBy, question.GeneratedBy);
        Assert.Equal(sourceReference, question.QuestionSourceReference);
        Assert.Empty(question.Options);
        Assert.Single(question.DomainEvents);
        Assert.IsType<QuestionCreatedEvent>(question.DomainEvents.First());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidQuestionText_ShouldThrowException(string questionText)
    {
        // Act & Assert
        Assert.Throws<DomainValidationException>(() =>
            new Question(questionText, 1, 2, 5, QuestionSource.Manual));
    }

    [Fact]
    public void Constructor_WithTooLongQuestionText_ShouldThrowException()
    {
        // Arrange
        var questionText = new string('a', 1001);

        // Act & Assert
        Assert.Throws<DomainValidationException>(() =>
            new Question(questionText, 1, 2, 5, QuestionSource.Manual));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Constructor_WithInvalidDifficultyLevel_ShouldThrowException(int difficultyLevel)
    {
        // Act & Assert
        Assert.Throws<DomainValidationException>(() =>
            new Question("Valid question?", 1, difficultyLevel, 5, QuestionSource.Manual));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public void Constructor_WithInvalidMaxScore_ShouldThrowException(int maxScore)
    {
        // Act & Assert
        Assert.Throws<DomainValidationException>(() =>
            new Question("Valid question?", 1, 2, maxScore, QuestionSource.Manual));
    }

    [Fact]
    public void Constructor_WithInvalidSourceReference_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() => 
            new Question("Valid question?", 1, 2, 5, QuestionSource.Manual, "not-a-valid-url"));
    }

    [Fact]
    public void UpdateQuestion_WithValidData_ShouldUpdateQuestion()
    {
        // Arrange
        var question = new Question("Original question?", 1, 2, 5, QuestionSource.Manual);
        var newText = "Updated question?";
        var newDifficulty = 3;
        var newMaxScore = 7;
        var newGeneratedBy = QuestionSource.AI;
        var newSourceReference = "https://example.com/updated";
        question.ClearDomainEvents();

        // Act
        question.UpdateQuestion(newText, newDifficulty, newMaxScore, newGeneratedBy, newSourceReference);

        // Assert
        Assert.Equal(newText, question.QuestionText);
        Assert.Equal(newDifficulty, question.DifficultyLevel);
        Assert.Equal(newMaxScore, question.MaxScore);
        Assert.Equal(newGeneratedBy, question.GeneratedBy);
        Assert.Equal(newSourceReference, question.QuestionSourceReference);
        Assert.Single(question.DomainEvents);
        Assert.IsType<QuestionUpdatedEvent>(question.DomainEvents.First());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateQuestion_WithInvalidQuestionText_ShouldThrowException(string questionText)
    {
        // Arrange
        var question = new Question("Original question?", 1, 2, 5, QuestionSource.Manual);

        // Act & Assert
        Assert.Throws<DomainValidationException>(() =>
            question.UpdateQuestion(questionText, 2, 5, QuestionSource.Manual));
    }

    [Fact]
    public void AddOption_WithValidData_ShouldAddOption()
    {
        // Arrange
        var question = new Question("What is 2 + 2?", 1, 2, 5, QuestionSource.Manual);
        var optionText = "4";
        var isCorrect = true;
        question.ClearDomainEvents();

        // Act
        var option = question.AddOption(optionText, isCorrect);

        // Assert
        Assert.Single(question.Options);
        Assert.Equal(optionText, option.OptionText);
        Assert.Equal(question.Id, option.QuestionId);
        Assert.Equal(isCorrect, option.IsCorrect);
        Assert.Single(question.DomainEvents);
        Assert.IsType<QuestionOptionAddedEvent>(question.DomainEvents.First());
    }

    [Fact]
    public void RemoveOption_WithExistingOption_ShouldRemoveOption()
    {
        // Arrange
        var question = new Question("What is 2 + 2?", 1, 2, 5, QuestionSource.Manual);
        var option = question.AddOption("4", true);
        question.ClearDomainEvents();

        // Act
        question.RemoveOption(option.Id);

        // Assert
        Assert.Empty(question.Options);
        Assert.Single(question.DomainEvents);
        Assert.IsType<QuestionOptionRemovedEvent>(question.DomainEvents.First());
    }

    [Fact]
    public void RemoveOption_WithNonExistingOption_ShouldThrowException()
    {
        // Arrange
        var question = new Question("What is 2 + 2?", 1, 2, 5, QuestionSource.Manual);

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() => question.RemoveOption(999));
    }

    [Fact]
    public void ValidateHasCorrectAnswer_WithCorrectAnswer_ShouldNotThrow()
    {
        // Arrange
        var question = new Question("What is 2 + 2?", 1, 2, 5, QuestionSource.Manual);
        question.AddOption("3", false);
        question.AddOption("4", true);
        question.AddOption("5", false);

        // Act & Assert
        question.ValidateHasCorrectAnswer(); // Should not throw
    }

    [Fact]
    public void ValidateHasCorrectAnswer_WithoutCorrectAnswer_ShouldThrowException()
    {
        // Arrange
        var question = new Question("What is 2 + 2?", 1, 2, 5, QuestionSource.Manual);
        question.AddOption("3", false);
        question.AddOption("5", false);

        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() => question.ValidateHasCorrectAnswer());
    }

    [Fact]
    public void ClearDomainEvents_ShouldClearAllEvents()
    {
        // Arrange
        var question = new Question("What is 2 + 2?", 1, 2, 5, QuestionSource.Manual);
        question.AddOption("4", true);

        // Act
        question.ClearDomainEvents();

        // Assert
        Assert.Empty(question.DomainEvents);
    }
}
