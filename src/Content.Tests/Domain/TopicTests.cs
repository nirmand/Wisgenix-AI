using Content.Domain.Entities;
using Content.Domain.Events;
using Wisgenix.SharedKernel.Domain.Exceptions;
using Wisgenix.SharedKernel.Domain.Enums;
using Xunit;

namespace Content.Tests.Domain;

public class TopicTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateTopic()
    {
        // Arrange
        var topicName = "Algebra";
        var subjectId = 1;

        // Act
        var topic = new Topic(topicName, subjectId);

        // Assert
        Assert.Equal(topicName, topic.TopicName);
        Assert.Equal(subjectId, topic.SubjectId);
        Assert.Empty(topic.Questions);
        Assert.Single(topic.DomainEvents);
        Assert.IsType<TopicCreatedEvent>(topic.DomainEvents.First());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidName_ShouldThrowException(string topicName)
    {
        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() => new Topic(topicName, 1));
    }

    [Fact]
    public void Constructor_WithTooLongName_ShouldThrowException()
    {
        // Arrange
        var topicName = new string('a', 201);

        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() => new Topic(topicName, 1));
    }

    [Theory]
    [InlineData("Topic>Name")]
    [InlineData("Topic<123")]
    [InlineData("Topic&Name")]
    [InlineData("Topic\"Name")]
    [InlineData("Topic'Name")]
    public void Constructor_WithInvalidCharacters_ShouldThrowException(string topicName)
    {
        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() => new Topic(topicName, 1));
    }

    [Fact]
    public void UpdateTopicName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var topic = new Topic("Algebra", 1);
        var newName = "Advanced Algebra";
        topic.ClearDomainEvents();

        // Act
        topic.UpdateTopicName(newName);

        // Assert
        Assert.Equal(newName, topic.TopicName);
        Assert.Single(topic.DomainEvents);
        Assert.IsType<TopicUpdatedEvent>(topic.DomainEvents.First());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateTopicName_WithInvalidName_ShouldThrowException(string newName)
    {
        // Arrange
        var topic = new Topic("Algebra", 1);

        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() => topic.UpdateTopicName(newName));
    }

    [Fact]
    public void AddQuestion_WithValidData_ShouldAddQuestion()
    {
        // Arrange
        var topic = new Topic("Algebra", 1);
        var questionText = "What is 2 + 2?";
        var difficultyLevel = 1;
        var maxScore = 5;
        var generatedBy = QuestionSource.Manual;
        topic.ClearDomainEvents();

        // Act
        var question = topic.AddQuestion(questionText, difficultyLevel, maxScore, generatedBy);

        // Assert
        Assert.Single(topic.Questions);
        Assert.Equal(questionText, question.QuestionText);
        Assert.Equal(topic.Id, question.TopicId);
        Assert.Equal(difficultyLevel, question.DifficultyLevel);
        Assert.Equal(maxScore, question.MaxScore);
        Assert.Equal(generatedBy, question.GeneratedBy);
        Assert.Single(topic.DomainEvents);
        Assert.IsType<QuestionAddedToTopicEvent>(topic.DomainEvents.First());
    }

    [Fact]
    public void AddQuestion_WithDuplicateText_ShouldThrowException()
    {
        // Arrange
        var topic = new Topic("Algebra", 1);
        var questionText = "What is 2 + 2?";
        topic.AddQuestion(questionText, 1, 5, QuestionSource.Manual);

        // Act & Assert
        Assert.Throws<DuplicateEntityException>(() => 
            topic.AddQuestion(questionText, 2, 3, QuestionSource.AI));
    }

    [Fact]
    public void AddQuestion_WithDuplicateTextDifferentCase_ShouldThrowException()
    {
        // Arrange
        var topic = new Topic("Algebra", 1);
        topic.AddQuestion("What is 2 + 2?", 1, 5, QuestionSource.Manual);

        // Act & Assert
        Assert.Throws<DuplicateEntityException>(() => 
            topic.AddQuestion("WHAT IS 2 + 2?", 2, 3, QuestionSource.AI));
    }

    [Fact]
    public void AddQuestion_WithSourceReference_ShouldAddQuestion()
    {
        // Arrange
        var topic = new Topic("Algebra", 1);
        var questionText = "What is 2 + 2?";
        var sourceReference = "https://example.com/source";
        topic.ClearDomainEvents();

        // Act
        var question = topic.AddQuestion(questionText, 1, 5, QuestionSource.Manual, sourceReference);

        // Assert
        Assert.Single(topic.Questions);
        Assert.Equal(sourceReference, question.QuestionSourceReference);
        Assert.Single(topic.DomainEvents);
        Assert.IsType<QuestionAddedToTopicEvent>(topic.DomainEvents.First());
    }

    [Fact]
    public void RemoveQuestion_WithExistingQuestion_ShouldRemoveQuestion()
    {
        // Arrange
        var topic = new Topic("Algebra", 1);
        var question = topic.AddQuestion("What is 2 + 2?", 1, 5, QuestionSource.Manual);
        topic.ClearDomainEvents();

        // Act
        topic.RemoveQuestion(question.Id);

        // Assert
        Assert.Empty(topic.Questions);
        Assert.Single(topic.DomainEvents);
        Assert.IsType<QuestionRemovedFromTopicEvent>(topic.DomainEvents.First());
    }

    [Fact]
    public void RemoveQuestion_WithNonExistingQuestion_ShouldThrowException()
    {
        // Arrange
        var topic = new Topic("Algebra", 1);

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() => topic.RemoveQuestion(999));
    }

    [Fact]
    public void ClearDomainEvents_ShouldClearAllEvents()
    {
        // Arrange
        var topic = new Topic("Algebra", 1);
        topic.AddQuestion("What is 2 + 2?", 1, 5, QuestionSource.Manual);

        // Act
        topic.ClearDomainEvents();

        // Assert
        Assert.Empty(topic.DomainEvents);
    }
}
