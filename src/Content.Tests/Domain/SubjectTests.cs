using Content.Domain.Entities;
using Content.Domain.Events;
using Wisgenix.SharedKernel.Exceptions;
using Xunit;

namespace Content.Tests.Domain;

public class SubjectTests
{
    [Fact]
    public void Constructor_WithValidName_ShouldCreateSubject()
    {
        // Arrange
        var subjectName = "Mathematics";

        // Act
        var subject = new Subject(subjectName);

        // Assert
        Assert.Equal(subjectName, subject.SubjectName);
        Assert.Empty(subject.Topics);
        Assert.Single(subject.DomainEvents);
        Assert.IsType<SubjectCreatedEvent>(subject.DomainEvents.First());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidName_ShouldThrowException(string subjectName)
    {
        // Act & Assert
        Assert.Throws<DomainValidationException>(() => new Subject(subjectName));
    }

    [Fact]
    public void Constructor_WithTooLongName_ShouldThrowException()
    {
        // Arrange
        var subjectName = new string('a', 201);

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => new Subject(subjectName));
    }

    [Theory]
    [InlineData("Subject>Name")]
    [InlineData("Subject<123")]
    [InlineData("Subject&Name")]
    [InlineData("Subject\"Name")]
    [InlineData("Subject'Name")]
    public void Constructor_WithInvalidCharacters_ShouldThrowException(string subjectName)
    {
        // Act & Assert
        Assert.Throws<DomainValidationException>(() => new Subject(subjectName));
    }

    [Fact]
    public void UpdateSubjectName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        var newName = "Advanced Mathematics";
        subject.ClearDomainEvents();

        // Act
        subject.UpdateSubjectName(newName);

        // Assert
        Assert.Equal(newName, subject.SubjectName);
        Assert.Single(subject.DomainEvents);
        Assert.IsType<SubjectUpdatedEvent>(subject.DomainEvents.First());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateSubjectName_WithInvalidName_ShouldThrowException(string newName)
    {
        // Arrange
        var subject = new Subject("Mathematics");

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => subject.UpdateSubjectName(newName));
    }

    [Fact]
    public void AddTopic_WithValidName_ShouldAddTopic()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        var topicName = "Algebra";
        subject.ClearDomainEvents();

        // Act
        var topic = subject.AddTopic(topicName);

        // Assert
        Assert.Single(subject.Topics);
        Assert.Equal(topicName, topic.TopicName);
        Assert.Equal(subject.Id, topic.SubjectId);
        Assert.Single(subject.DomainEvents);
        Assert.IsType<TopicAddedToSubjectEvent>(subject.DomainEvents.First());
    }

    [Fact]
    public void AddTopic_WithDuplicateName_ShouldThrowException()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        var topicName = "Algebra";
        subject.AddTopic(topicName);

        // Act & Assert
        Assert.Throws<DuplicateEntityException>(() => subject.AddTopic(topicName));
    }

    [Fact]
    public void AddTopic_WithDuplicateNameDifferentCase_ShouldThrowException()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        subject.AddTopic("Algebra");

        // Act & Assert
        Assert.Throws<DuplicateEntityException>(() => subject.AddTopic("ALGEBRA"));
    }

    [Fact]
    public void RemoveTopic_WithExistingTopic_ShouldRemoveTopic()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        var topic = subject.AddTopic("Algebra");
        subject.ClearDomainEvents();

        // Act
        subject.RemoveTopic(topic.Id);

        // Assert
        Assert.Empty(subject.Topics);
        Assert.Single(subject.DomainEvents);
        Assert.IsType<TopicRemovedFromSubjectEvent>(subject.DomainEvents.First());
    }

    [Fact]
    public void RemoveTopic_WithNonExistingTopic_ShouldThrowException()
    {
        // Arrange
        var subject = new Subject("Mathematics");

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() => subject.RemoveTopic(999));
    }

    [Fact]
    public void ClearDomainEvents_ShouldClearAllEvents()
    {
        // Arrange
        var subject = new Subject("Mathematics");
        subject.AddTopic("Algebra");

        // Act
        subject.ClearDomainEvents();

        // Assert
        Assert.Empty(subject.DomainEvents);
    }
}
