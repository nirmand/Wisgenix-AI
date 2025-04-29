using AIUpskillingPlatform.API.Controllers;
using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using AIUpskillingPlatform.DTO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using AIUpskillingPlatform.Core.Logger;
using AutoMapper;

namespace AIUpskillingPlatform.Tests.Controllers;

public class TopicsControllerTests
{
    private readonly Mock<ITopicRepository> _mockRepository;
    private readonly Mock<ISubjectRepository> _mockSubjectRepository;
    private readonly Mock<ILoggingService> _mockLogger;
    private readonly TopicsController _controller;
    private readonly Mock<IMapper> _mapper;
    private readonly LogContext _logContext;

    public TopicsControllerTests()
    {
        _mockRepository = new Mock<ITopicRepository>();        
        _mockLogger = new Mock<ILoggingService>();
        _mockSubjectRepository = new Mock<ISubjectRepository>();
        _mapper = new Mock<IMapper>(); 
        _controller = new TopicsController(_mockRepository.Object, _mockSubjectRepository.Object, _mockLogger.Object, _mapper.Object);
        _logContext = LogContext.Create("TestContext");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CreateTopic_WithInvalidName_ReturnsBadRequest(string topicName)
    {
        // Arrange
        var createDto = new CreateTopicDto { TopicName = topicName, SubjectID = 1 };
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(_logContext, 1)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<Topic>(It.IsAny<CreateTopicDto>()))
            .Returns(new Topic { TopicName = topicName, SubjectID = 1 });

        // Act
        var result = await _controller.CreateTopic(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Topic name is required", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateTopic_WithTooLongName_ReturnsBadRequest()
    {
        // Arrange
        string longName = new string('a', 101); // Create a string longer than 100 characters
        var createDto = new CreateTopicDto { TopicName = longName, SubjectID = 1 };
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(_logContext, 1)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<Topic>(It.IsAny<CreateTopicDto>()))
            .Returns(new Topic { TopicName = longName, SubjectID = 1 });

        // Act
        var result = await _controller.CreateTopic(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Topic name must not exceed 100 characters", badRequestResult.Value);
    }

    [Theory]
    [InlineData("Topic@Name")]
    [InlineData("Topic#123")]
    [InlineData("Topic-Name")]
    public async Task CreateTopic_WithInvalidCharacters_ReturnsBadRequest(string topicName)
    {
        // Arrange
        var createDto = new CreateTopicDto { TopicName = topicName, SubjectID = 1 };
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(_logContext, 1)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<Topic>(It.IsAny<CreateTopicDto>()))
            .Returns(new Topic { TopicName = topicName, SubjectID = 1 });

        // Act
        var result = await _controller.CreateTopic(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Topic name contains invalid characters", badRequestResult.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateTopic_WithInvalidSubjectId_ReturnsBadRequest(int subjectId)
    {
        // Arrange
        var createDto = new CreateTopicDto { TopicName = "Valid Topic", SubjectID = subjectId };
        _mapper.Setup(m => m.Map<Topic>(It.IsAny<CreateTopicDto>()))
            .Returns(new Topic { TopicName = "Valid Topic", SubjectID = subjectId });

        // Act
        var result = await _controller.CreateTopic(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Subject ID must be greater than 0", badRequestResult.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task UpdateTopic_WithInvalidName_ReturnsBadRequest(string topicName)
    {
        // Arrange
        var updateDto = new UpdateTopicDto { TopicName = topicName, SubjectID = 1 };
        var existingTopic = new Topic { ID = 1, TopicName = "Original Topic", SubjectID = 1 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(_logContext, 1)).ReturnsAsync(existingTopic);
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(_logContext, 1)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map(It.IsAny<UpdateTopicDto>(), It.IsAny<Topic>()))
            .Callback<UpdateTopicDto, Topic>((dto, topic) => {
                topic.TopicName = dto.TopicName;
                topic.SubjectID = dto.SubjectID;
            });

        // Act
        var result = await _controller.UpdateTopic(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Topic name is required", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateTopic_WithTooLongName_ReturnsBadRequest()
    {
        // Arrange
        string longName = new string('a', 101);
        var updateDto = new UpdateTopicDto { TopicName = longName, SubjectID = 1 };
        var existingTopic = new Topic { ID = 1, TopicName = "Original Topic", SubjectID = 1 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(_logContext, 1)).ReturnsAsync(existingTopic);
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(_logContext, 1)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map(It.IsAny<UpdateTopicDto>(), It.IsAny<Topic>()))
            .Callback<UpdateTopicDto, Topic>((dto, topic) => {
                topic.TopicName = dto.TopicName;
                topic.SubjectID = dto.SubjectID;
            });

        // Act
        var result = await _controller.UpdateTopic(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Topic name must not exceed 100 characters", badRequestResult.Value);
    }

    [Theory]
    [InlineData("Topic@Name")]
    [InlineData("Topic#123")]
    [InlineData("Topic-Name")]
    public async Task UpdateTopic_WithInvalidCharacters_ReturnsBadRequest(string topicName)
    {
        // Arrange
        var updateDto = new UpdateTopicDto { TopicName = topicName, SubjectID = 1 };
        var existingTopic = new Topic { ID = 1, TopicName = "Original Topic", SubjectID = 1 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(_logContext, 1)).ReturnsAsync(existingTopic);
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(_logContext, 1)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map(It.IsAny<UpdateTopicDto>(), It.IsAny<Topic>()))
            .Callback<UpdateTopicDto, Topic>((dto, topic) => {
                topic.TopicName = dto.TopicName;
                topic.SubjectID = dto.SubjectID;
            });

        // Act
        var result = await _controller.UpdateTopic(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Topic name contains invalid characters", badRequestResult.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task UpdateTopic_WithInvalidSubjectId_ReturnsBadRequest(int subjectId)
    {
        // Arrange
        var updateDto = new UpdateTopicDto { TopicName = "Valid Topic", SubjectID = subjectId };
        var existingTopic = new Topic { ID = 1, TopicName = "Original Topic", SubjectID = 1 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(_logContext, 1)).ReturnsAsync(existingTopic);
        _mapper.Setup(m => m.Map(It.IsAny<UpdateTopicDto>(), It.IsAny<Topic>()))
            .Callback<UpdateTopicDto, Topic>((dto, topic) => {
                topic.TopicName = dto.TopicName;
                topic.SubjectID = dto.SubjectID;
            });

        // Act
        var result = await _controller.UpdateTopic(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Subject ID must be greater than 0", badRequestResult.Value);
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
    public async Task GetTopic_ReturnsOkResult_WithTopic()
    {
        // Arrange
        var topic = new Topic { ID = 1, TopicName = "Test Topic" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(_logContext, 1))
            .ReturnsAsync(topic);

        // Act
        var result = await _controller.GetTopic(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTopic = Assert.IsType<TopicDto>(okResult.Value);
        Assert.Equal(topic.ID, returnedTopic.ID);
        Assert.Equal(topic.TopicName, returnedTopic.TopicName);
        _mockRepository.Verify(repo => repo.GetByIdAsync(_logContext, 1), Times.Once);
    }

    [Fact]
    public async Task GetTopic_ReturnsNotFound_WhenTopicDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(_logContext, 1))
            .ThrowsAsync(new TopicNotFoundException(1));

        // Act
        var result = await _controller.GetTopic(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Topic with ID 1 was not found.", notFoundResult.Value);
        _mockRepository.Verify(repo => repo.GetByIdAsync(_logContext, 1), Times.Once);
    }

    [Fact]
    public async Task DeleteTopic_ReturnsNoContent_WhenTopicExists()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(_logContext, 1))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteTopic(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.DeleteAsync(_logContext, 1), Times.Once);
    }

    [Fact]
    public async Task DeleteTopic_ReturnsNotFound_WhenTopicDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(_logContext, 1))
            .ThrowsAsync(new TopicNotFoundException(1));

        // Act
        var result = await _controller.DeleteTopic(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Topic with ID 1 was not found.", notFoundResult.Value);
        _mockRepository.Verify(repo => repo.DeleteAsync(_logContext, 1), Times.Once);
    }
}