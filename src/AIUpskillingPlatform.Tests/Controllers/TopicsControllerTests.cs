using AIUpskillingPlatform.API.Controllers;
using AIUpskillingPlatform.API.DTOs;
using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using AIUpskillingPlatform.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AIUpskillingPlatform.Core.Logger;

namespace AIUpskillingPlatform.Tests.Controllers;

public class TopicsControllerTests
{
    private readonly Mock<ITopicRepository> _mockRepository;
    private readonly Mock<ILoggingService> _mockLogger;
    private readonly TopicsController _controller;

    private readonly LogContext _logContext;

    public TopicsControllerTests()
    {
        _mockRepository = new Mock<ITopicRepository>();
        _mockLogger = new Mock<ILoggingService>();
        _controller = new TopicsController(_mockRepository.Object, _mockLogger.Object);
        _logContext = LogContext.Create("TestContext");
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
        _mockRepository.Setup(repo => repo.GetAllAsync(_logContext))
            .ReturnsAsync(topics);

        // Act
        var result = await _controller.GetTopics();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTopics = Assert.IsAssignableFrom<IEnumerable<TopicDto>>(okResult.Value);
        Assert.Equal(2, returnedTopics.Count());
        _mockRepository.Verify(repo => repo.GetAllAsync(_logContext), Times.Once);
    }

    [Fact]
    public async Task GetTopics_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetAllAsync(_logContext))
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
        _mockRepository.Setup(repo => repo.GetByIdAsync(_logContext,1))
            .ReturnsAsync(topic);

        // Act
        var result = await _controller.GetTopic(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTopic = Assert.IsType<TopicDto>(okResult.Value);
        Assert.Equal(topic.ID, returnedTopic.ID);
        Assert.Equal(topic.TopicName, returnedTopic.TopicName);
        _mockRepository.Verify(repo => repo.GetByIdAsync(_logContext,1), Times.Once);
    }

    [Fact]
    public async Task GetTopic_ReturnsNotFound_WhenTopicDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(_logContext,1))
            .ThrowsAsync(new TopicNotFoundException(1));

        // Act
        var result = await _controller.GetTopic(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Topic with ID 1 was not found.", notFoundResult.Value);
        _mockRepository.Verify(repo => repo.GetByIdAsync(_logContext,1), Times.Once);
    }

    [Fact]
    public async Task GetTopic_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(_logContext,1))
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
        var createTopicDto = new CreateTopicDto { TopicName = "New Topic", SubjectID = 1 };
        var createdTopic = new Topic { ID = 1, TopicName = "New Topic", SubjectID = 1 };
        _mockRepository.Setup(repo => repo.SubjectExistsAsync(_logContext,1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.CreateAsync(_logContext,It.IsAny<Topic>()))
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
        _mockRepository.Verify(repo => repo.CreateAsync(_logContext,It.IsAny<Topic>()), Times.Once);
    }

    [Fact]
    public async Task CreateTopic_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        var createTopicDto = new CreateTopicDto { TopicName = "New Topic", SubjectID = 1 };
        _mockRepository.Setup(repo => repo.SubjectExistsAsync(_logContext,1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.CreateAsync(_logContext,It.IsAny<Topic>()))
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
        var originalTopic = new Topic { ID = 1, TopicName = "Original Topic", SubjectID = 1 };
        var updatedTopic = new Topic { ID = 1, TopicName = "Updated Topic", SubjectID = 1 };
        var updateTopicDto = new UpdateTopicDto { TopicName = "Updated Topic", SubjectID = 1 };
        _mockRepository.Setup(repo => repo.SubjectExistsAsync(_logContext,1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.GetByIdAsync(_logContext,1)).ReturnsAsync(originalTopic);
        _mockRepository.Setup(repo => repo.UpdateAsync(_logContext,It.IsAny<Topic>()))
            .ReturnsAsync(updatedTopic);

        // Act
        var result = await _controller.UpdateTopic(1, updateTopicDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.UpdateAsync(_logContext,It.IsAny<Topic>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTopic_ReturnsNotFound_WhenTopicDoesNotExist()
    {
        // Arrange
        var updateTopicDto = new UpdateTopicDto { TopicName = "Updated Topic", SubjectID = 1 };
        _mockRepository.Setup(repo => repo.SubjectExistsAsync(_logContext,1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.UpdateAsync(_logContext,It.IsAny<Topic>()))
            .ThrowsAsync(new TopicNotFoundException(1));

        // Act
        var result = await _controller.UpdateTopic(1, updateTopicDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Topic with ID 1 was not found.", notFoundResult.Value);
        _mockRepository.Verify(repo => repo.UpdateAsync(_logContext,It.IsAny<Topic>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTopic_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        var originalTopic = new Topic { ID = 1, TopicName = "Original Topic", SubjectID = 1 };
        var updateTopicDto = new UpdateTopicDto { TopicName = "Updated Topic", SubjectID = 1 };
        _mockRepository.Setup(repo => repo.SubjectExistsAsync(_logContext,1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.GetByIdAsync(_logContext,1)).ReturnsAsync(originalTopic);
        _mockRepository.Setup(repo => repo.UpdateAsync(_logContext,It.IsAny<Topic>()))
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
        _mockRepository.Setup(repo => repo.DeleteAsync(_logContext,1))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteTopic(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.DeleteAsync(_logContext,1), Times.Once);
    }

    [Fact]
    public async Task DeleteTopic_ReturnsNotFound_WhenTopicDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(_logContext,1))
            .ThrowsAsync(new TopicNotFoundException(1));

        // Act
        var result = await _controller.DeleteTopic(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Topic with ID 1 was not found.", notFoundResult.Value);
        _mockRepository.Verify(repo => repo.DeleteAsync(_logContext,1), Times.Once);
    }

    [Fact]
    public async Task DeleteTopic_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(_logContext,1))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.DeleteTopic(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while deleting the topic", statusCodeResult.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("A")]
    [InlineData("This is a very long topic name that exceeds the maximum length of 200 characters allowed by the system. This should be rejected by the validation.")]
    public async Task CreateTopic_ValidatesTopicName(string topicName)
    {
        // Arrange
        var createTopicDto = new CreateTopicDto { TopicName = topicName, SubjectID = 1 };
        var createdTopic = new Topic { ID = 1, TopicName = topicName, SubjectID = 1 };
        _mockRepository.Setup(repo => repo.SubjectExistsAsync(_logContext,1)).ReturnsAsync(true);
        if (!string.IsNullOrEmpty(topicName))
        {
            _mockRepository.Setup(repo => repo.CreateAsync(_logContext,It.IsAny<Topic>()))
                .ReturnsAsync(createdTopic);
        }
        else
        {
            _mockRepository.Setup(repo => repo.CreateAsync(_logContext,It.IsAny<Topic>()));
        }

        // Act
        var result = await _controller.CreateTopic(createTopicDto);

        // Assert
        if (string.IsNullOrWhiteSpace(topicName))
        {
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Topic name is required", badRequestResult.Value);
        }
        else
        {
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedTopic = Assert.IsType<TopicDto>(createdResult.Value);
            Assert.Equal(createdTopic.ID, returnedTopic.ID);
            Assert.Equal(createdTopic.TopicName, returnedTopic.TopicName);
            Assert.Equal(nameof(TopicsController.GetTopic), createdResult.ActionName);
            Assert.Equal(createdTopic.ID, createdResult.RouteValues["id"]);
            _mockRepository.Verify(repo => repo.CreateAsync(_logContext,It.IsAny<Topic>()), Times.Once);
        }
    }

    [Fact]
    public async Task CreateTopic_WithoutSubject_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateTopicDto 
        { 
            TopicName = "Test Topic",
            SubjectID = 0  // Invalid Subject ID
        };
        _mockRepository.Setup(repo => repo.SubjectExistsAsync(_logContext,0)).ReturnsAsync(false);

        // Act
        var result = await _controller.CreateTopic(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Subject ID is required", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateTopic_WithInvalidSubject_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateTopicDto 
        { 
            TopicName = "Test Topic",
            SubjectID = 999  // Non-existent Subject ID
        };
        _mockRepository.Setup(repo => repo.SubjectExistsAsync(_logContext,999)).ReturnsAsync(false);

        // Act
        var result = await _controller.CreateTopic(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Subject with ID 999 does not exist", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateTopic_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateTopicDto 
        { 
            TopicName = "Test Topic",
            SubjectID = 1  // Added SubjectID
        };
        _mockRepository.Setup(repo => repo.SubjectExistsAsync(_logContext,1)).ReturnsAsync(true);  // Added Subject validation
        _mockRepository.Setup(repo => repo.CreateAsync(_logContext,It.IsAny<Topic>()))
            .ReturnsAsync(new Topic 
            { 
                ID = 1, 
                TopicName = createDto.TopicName,
                SubjectID = createDto.SubjectID  // Added SubjectID
            });

        // Act
        var result = await _controller.CreateTopic(createDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedTopic = Assert.IsType<TopicDto>(createdAtActionResult.Value);
        Assert.Equal(createDto.TopicName, returnedTopic.TopicName);
        Assert.Equal(createDto.SubjectID, returnedTopic.SubjectID);  // Added SubjectID assertion
    }

    [Fact]
    public async Task CreateTopic_WithInvalidSubjectId_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateTopicDto 
        { 
            TopicName = "Test Topic",
            SubjectID = 999
        };
        _mockRepository.Setup(repo => repo.SubjectExistsAsync(_logContext,999)).ReturnsAsync(false);

        // Act
        var result = await _controller.CreateTopic(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Subject with ID 999 does not exist", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateTopic_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var updateDto = new UpdateTopicDto 
        { 
            TopicName = "Updated Topic",
            SubjectID = 1  // Added SubjectID
        };
        var existingTopic = new Topic { ID = 1, TopicName = "Test Topic", SubjectID = 1 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(_logContext,1)).ReturnsAsync(existingTopic);
        _mockRepository.Setup(repo => repo.SubjectExistsAsync(_logContext,1)).ReturnsAsync(true);  // Added Subject validation

        // Act
        var result = await _controller.UpdateTopic(1, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.UpdateAsync(_logContext,It.Is<Topic>(t => 
            t.TopicName == updateDto.TopicName && 
            t.SubjectID == updateDto.SubjectID)), 
            Times.Once);
    }

    [Fact]
    public async Task UpdateTopic_WithInvalidSubjectId_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateTopicDto 
        { 
            TopicName = "Updated Topic",
            SubjectID = 999
        };
        _mockRepository.Setup(repo => repo.SubjectExistsAsync(_logContext,999)).ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateTopic(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Subject with ID 999 does not exist", badRequestResult.Value);
    }

    [Fact]
    public async Task GetTopics_ReturnsOkResult_WithTopics()
    {
        // Arrange
        var topics = new List<Topic>
        {
            new Topic 
            { 
                ID = 1, 
                TopicName = "Test Topic", 
                SubjectID = 1,  // Added SubjectID
                Subject = new Subject { ID = 1, SubjectName = "Test Subject" }  // Added Subject
            }
        };
        _mockRepository.Setup(repo => repo.GetAllAsync(_logContext)).ReturnsAsync(topics);

        // Act
        var result = await _controller.GetTopics();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTopics = Assert.IsAssignableFrom<IEnumerable<TopicDto>>(okResult.Value);
        var topic = returnedTopics.First();
        Assert.Equal("Test Topic", topic.TopicName);
        Assert.Equal(1, topic.SubjectID);  // Added SubjectID assertion
    }

    [Fact]
    public async Task GetTopic_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var topic = new Topic 
        { 
            ID = 1, 
            TopicName = "Test Topic",
            SubjectID = 1,  // Added SubjectID
            Subject = new Subject { ID = 1, SubjectName = "Test Subject" }  // Added Subject
        };
        _mockRepository.Setup(repo => repo.GetByIdAsync(_logContext,1)).ReturnsAsync(topic);

        // Act
        var result = await _controller.GetTopic(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTopic = Assert.IsType<TopicDto>(okResult.Value);
        Assert.Equal(topic.TopicName, returnedTopic.TopicName);
        Assert.Equal(topic.SubjectID, returnedTopic.SubjectID);  // Added SubjectID assertion
    }
}