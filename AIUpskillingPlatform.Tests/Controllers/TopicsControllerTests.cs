using AIUpskillingPlatform.API.Controllers;
using AIUpskillingPlatform.API.DTOs;
using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AIUpskillingPlatform.Tests.Controllers;

public class TopicsControllerTests
{
    private readonly Mock<ITopicRepository> _mockRepository;
    private readonly Mock<ILogger<TopicsController>> _mockLogger;
    private readonly TopicsController _controller;

    public TopicsControllerTests()
    {
        _mockRepository = new Mock<ITopicRepository>();
        _mockLogger = new Mock<ILogger<TopicsController>>();
        _controller = new TopicsController(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetTopics_ReturnsOkResult_WithListOfTopics()
    {
        // Arrange
        var topics = new List<Topic>
        {
            new() { ID = 1, TopicName = "Topic 1" },
            new() { ID = 2, TopicName = "Topic 2" }
        };
        _mockRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(topics);

        // Act
        var result = await _controller.GetTopics();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTopics = Assert.IsAssignableFrom<IEnumerable<TopicDto>>(okResult.Value);
        Assert.Equal(2, returnedTopics.Count());
        _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTopics_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetAllAsync())
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetTopics();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving topics", statusCodeResult.Value);
    }

    [Fact]
    public async Task GetTopic_ReturnsOkResult_WithTopic()
    {
        // Arrange
        var topic = new Topic { ID = 1, TopicName = "Test Topic" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(topic);

        // Act
        var result = await _controller.GetTopic(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTopic = Assert.IsType<TopicDto>(okResult.Value);
        Assert.Equal(topic.ID, returnedTopic.ID);
        Assert.Equal(topic.TopicName, returnedTopic.TopicName);
        _mockRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetTopic_ReturnsNotFound_WhenTopicDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(1))
            .ThrowsAsync(new TopicNotFoundException(1));

        // Act
        var result = await _controller.GetTopic(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Topic with ID 1 was not found.", notFoundResult.Value);
        _mockRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetTopic_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(1))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetTopic(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving the topic", statusCodeResult.Value);
    }

    [Fact]
    public async Task CreateTopic_ReturnsCreatedResult_WithTopic()
    {
        // Arrange
        var createTopicDto = new CreateTopicDto { TopicName = "New Topic" };
        var createdTopic = new Topic { ID = 1, TopicName = "New Topic" };
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Topic>()))
            .ReturnsAsync(createdTopic);

        // Act
        var result = await _controller.CreateTopic(createTopicDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedTopic = Assert.IsType<TopicDto>(createdResult.Value);
        Assert.Equal(createdTopic.ID, returnedTopic.ID);
        Assert.Equal(createdTopic.TopicName, returnedTopic.TopicName);
        Assert.Equal(nameof(TopicsController.GetTopic), createdResult.ActionName);
        Assert.Equal(createdTopic.ID, createdResult.RouteValues["id"]);
        _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<Topic>()), Times.Once);
    }

    [Fact]
    public async Task CreateTopic_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        var createTopicDto = new CreateTopicDto { TopicName = "New Topic" };
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Topic>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.CreateTopic(createTopicDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while creating the topic", statusCodeResult.Value);
    }

    [Fact]
    public async Task UpdateTopic_ReturnsNoContent_WhenTopicExists()
    {
        // Arrange
        var updateTopicDto = new UpdateTopicDto { TopicName = "Updated Topic" };
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Topic>()))
            .ReturnsAsync(new Topic { ID = 1, TopicName = "Updated Topic" });

        // Act
        var result = await _controller.UpdateTopic(1, updateTopicDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Topic>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTopic_ReturnsNotFound_WhenTopicDoesNotExist()
    {
        // Arrange
        var updateTopicDto = new UpdateTopicDto { TopicName = "Updated Topic" };
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Topic>()))
            .ThrowsAsync(new TopicNotFoundException(1));

        // Act
        var result = await _controller.UpdateTopic(1, updateTopicDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Topic with ID 1 was not found.", notFoundResult.Value);
        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Topic>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTopic_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        var updateTopicDto = new UpdateTopicDto { TopicName = "Updated Topic" };
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Topic>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.UpdateTopic(1, updateTopicDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while updating the topic", statusCodeResult.Value);
    }

    [Fact]
    public async Task DeleteTopic_ReturnsNoContent_WhenTopicExists()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(1))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteTopic(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteTopic_ReturnsNotFound_WhenTopicDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(1))
            .ThrowsAsync(new TopicNotFoundException(1));

        // Act
        var result = await _controller.DeleteTopic(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Topic with ID 1 was not found.", notFoundResult.Value);
        _mockRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteTopic_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(1))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.DeleteTopic(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while deleting the topic", statusCodeResult.Value);
    }

    [Theory]
    //[InlineData("")]
    //[InlineData(null)]
    [InlineData("A")]
    [InlineData("This is a very long topic name that exceeds the maximum length of 200 characters allowed by the system. This should be rejected by the validation.")]
    public async Task CreateTopic_ValidatesTopicName(string topicName)
    {
        // Arrange
    var createTopicDto = new CreateTopicDto { TopicName = topicName };
    var createdTopic = new Topic { ID = 1, TopicName = topicName };

    if (!string.IsNullOrEmpty(topicName) && topicName.Length <= 200)
    {
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Topic>()))
            .ReturnsAsync(createdTopic);
    }
    else
    {
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Topic>()));
    }

    // Act
    var result = await _controller.CreateTopic(createTopicDto);

    // Assert
        if (string.IsNullOrEmpty(topicName) || topicName.Length > 200)
        {
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid topic name", badRequestResult.Value);
        }
        else
        {
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedTopic = Assert.IsType<TopicDto>(createdResult.Value);
            Assert.Equal(createdTopic.ID, returnedTopic.ID);
            Assert.Equal(createdTopic.TopicName, returnedTopic.TopicName);
            Assert.Equal(nameof(TopicsController.GetTopic), createdResult.ActionName);
            Assert.Equal(createdTopic.ID, createdResult.RouteValues["id"]);
            _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<Topic>()), Times.Once);
        }
    }
} 