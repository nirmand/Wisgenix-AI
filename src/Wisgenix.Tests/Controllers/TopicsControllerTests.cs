using AutoMapper;
using Wisgenix.API.Controllers;
using Wisgenix.Data.Entities;
using Wisgenix.Repositories.Interfaces;
using Wisgenix.Core.Logger;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Wisgenix.DTO;
using Wisgenix.Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Wisgenix.Tests.Controllers;

public class TopicsControllerTests
{
    private readonly Mock<ITopicRepository> _mockRepository;
    private readonly Mock<ISubjectRepository> _mockSubjectRepository;
    private readonly Mock<ILoggingService> _mockLogger;
    private readonly TopicsController _controller;
    private readonly Mock<IMapper> _mapper;

    public TopicsControllerTests()
    {
        _mockRepository = new Mock<ITopicRepository>();
        _mockSubjectRepository = new Mock<ISubjectRepository>();
        _mockLogger = new Mock<ILoggingService>();
        _mapper = new Mock<IMapper>();
        _controller = new TopicsController(_mockRepository.Object, _mockSubjectRepository.Object, _mockLogger.Object, _mapper.Object);

        // Setup ControllerContext with HttpContext and ClaimsPrincipal
        var user = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "testuser")
            }, "mock"));
        var httpContext = new DefaultHttpContext { User = user };
        httpContext.Items["CorrelationId"] = "test-correlation-id";
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        _mapper.Setup(m => m.Map<TopicDto>(It.IsAny<Topic>()))
            .Returns((Topic source) => new TopicDto 
            { 
                ID = source.ID, 
                TopicName = source.TopicName,
                SubjectID = source.SubjectID,
                SubjectName = source.Subject?.SubjectName ?? string.Empty,
                CreatedDate = source.CreatedDate,
                CreatedBy = source.CreatedBy,
                ModifiedDate = source.ModifiedDate,
                ModifiedBy = source.ModifiedBy
            });
        
        _mapper.Setup(m => m.Map<IEnumerable<TopicDto>>(It.IsAny<IEnumerable<Topic>>()))
            .Returns((IEnumerable<Topic> source) => source.Select(t => new TopicDto 
            { 
                ID = t.ID, 
                TopicName = t.TopicName,
                SubjectID = t.SubjectID,
                SubjectName = t.Subject?.SubjectName ?? string.Empty,
                CreatedDate = t.CreatedDate,
                CreatedBy = t.CreatedBy,
                ModifiedDate = t.ModifiedDate,
                ModifiedBy = t.ModifiedBy
            }));

        _mapper.Setup(m => m.Map<Topic>(It.IsAny<CreateTopicDto>()))
            .Returns((CreateTopicDto source) => new Topic 
            { 
                TopicName = source.TopicName,
                SubjectID = source.SubjectID
            });
    }

    [Fact]
    public async Task GetTopics_ReturnsOkResult_WithTopics()
    {
        // Arrange
        var topics = new List<Topic>
        {
            new Topic { ID = 1, TopicName = "Classes", SubjectID = 1 },
            new Topic { ID = 2, TopicName = "Methods", SubjectID = 1 }
        };
        _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<LogContext>())).ReturnsAsync(topics);

        // Act
        var result = await _controller.GetTopics();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTopics = Assert.IsAssignableFrom<IEnumerable<TopicDto>>(okResult.Value);
        Assert.Equal(2, returnedTopics.Count());
    }

    [Fact]
    public async Task GetTopic_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var topic = new Topic { ID = 1, TopicName = "Classes", SubjectID = 1 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(topic);

        // Act
        var result = await _controller.GetTopic(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTopic = Assert.IsType<TopicDto>(okResult.Value);
        Assert.Equal(topic.TopicName, returnedTopic.TopicName);
    }

    [Fact]
    public async Task GetTopic_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        int invalidId = 999;
        string expectedErrorMessage = $"Topic with ID {invalidId} was not found";
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), invalidId))
            .ThrowsAsync(new TopicNotFoundException(invalidId));
        // Act
        var result = await _controller.GetTopic(invalidId);
        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.NotNull(notFoundResult.Value);
        Assert.Equal(expectedErrorMessage, notFoundResult.Value.ToString());
        _mockRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), invalidId), Times.Once);
    }

    [Fact]
    public async Task CreateTopic_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var userName = "testuser";
        var createDto = new CreateTopicDto { TopicName = "Classes", SubjectID = 1 };
        var createdTopic = new Topic {
            ID = 1,
            TopicName = "Classes",
            SubjectID = 1,
            CreatedDate = now,
            CreatedBy = userName,
            ModifiedDate = null,
            ModifiedBy = null
        };
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<LogContext>(), It.IsAny<Topic>())).ReturnsAsync(createdTopic);
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(It.IsAny<LogContext>(),1)).ReturnsAsync(true);


        // Act
        var result = await _controller.CreateTopic(createDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedTopic = Assert.IsType<TopicDto>(createdAtActionResult.Value);
        Assert.Equal(createDto.TopicName, returnedTopic.TopicName);
        Assert.Equal(now, returnedTopic.CreatedDate);
        Assert.Equal(userName, returnedTopic.CreatedBy);
        Assert.Null(returnedTopic.ModifiedDate);
        Assert.Null(returnedTopic.ModifiedBy);
    }

    [Fact]
    public async Task UpdateTopic_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var userName = "testuser";
        var updateDto = new UpdateTopicDto { TopicName = "Updated Classes", SubjectID = 1 };
        var existingTopic = new Topic {
            ID = 1,
            TopicName = "Classes",
            SubjectID = 1,
            CreatedDate = now.AddDays(-1),
            CreatedBy = userName,
            ModifiedDate = null,
            ModifiedBy = null
        };
        var updatedTopic = new Topic {
            ID = 1,
            TopicName = "Updated Classes",
            SubjectID = 1,
            CreatedDate = now.AddDays(-1),
            CreatedBy = userName,
            ModifiedDate = now,
            ModifiedBy = userName
        };
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(existingTopic);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<LogContext>(), It.IsAny<Topic>())).ReturnsAsync(updatedTopic);
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(It.IsAny<LogContext>(), existingTopic.SubjectID)).ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateTopic(1, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<LogContext>(), It.IsAny<Topic>()), Times.Once);
        // Optionally, verify audit fields on updatedTopic
        Assert.Equal(now, updatedTopic.ModifiedDate);
        Assert.Equal(userName, updatedTopic.ModifiedBy);
    }

    [Fact]
    public async Task UpdateTopic_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateTopicDto { TopicName = "Updated Classes" };
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(It.IsAny<LogContext>(),It.IsAny<int>())).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 999)).ReturnsAsync((Topic)null);

        // Act
        var result = await _controller.UpdateTopic(999, updateDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteTopic_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var topic = new Topic { ID = 1, TopicName = "Classes", SubjectID = 1 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1))
            .ReturnsAsync(topic);

        // Act
        var result = await _controller.DeleteTopic(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.DeleteAsync(It.IsAny<LogContext>(), 1), Times.Once);
    }

    [Fact]
    public async Task DeleteTopic_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        int invalidId =999;
        _mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<LogContext>(), invalidId)).ThrowsAsync(new TopicNotFoundException(invalidId));

        // Act
        var result = await _controller.DeleteTopic(invalidId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetTopics_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<LogContext>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetTopics();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving topics", statusCodeResult.Value);
    }

    [Fact]
    public async Task CreateTopic_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        var createDto = new CreateTopicDto { TopicName = "Test Topic", SubjectID = 1 };
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(It.IsAny<LogContext>(), 1))
            .ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<LogContext>(), It.IsAny<Topic>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.CreateTopic(createDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while creating the topic", statusCodeResult.Value);
    }

    [Fact]
    public async Task CreateTopic_WithInvalidSubjectId_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateTopicDto { TopicName = "Test Topic", SubjectID = 1 };
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(It.IsAny<LogContext>(), 1))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CreateTopic(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var errorObj = Assert.IsType<Newtonsoft.Json.Linq.JObject>(Newtonsoft.Json.Linq.JObject.FromObject(badRequestResult.Value));
        Assert.Equal($"Subject with ID {createDto.SubjectID} was not found", errorObj["message"]?.ToString());
    }

    [Fact]
    public async Task UpdateTopic_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateTopicDto();
        _controller.ModelState.AddModelError("TopicName", "Topic name is required");

        // Act
        var result = await _controller.UpdateTopic(1, updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateTopic_WithInvalidSubjectId_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateTopicDto { TopicName = "Test Topic", SubjectID = 1 };
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(It.IsAny<LogContext>(), 1))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateTopic(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorObj = Assert.IsType<Newtonsoft.Json.Linq.JObject>(Newtonsoft.Json.Linq.JObject.FromObject(badRequestResult.Value));
        Assert.Equal($"Subject with ID {updateDto.SubjectID} was not found", errorObj["message"]?.ToString());
    }

    [Fact]
    public async Task UpdateTopic_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        var updateDto = new UpdateTopicDto { TopicName = "Test Topic", SubjectID = 1 };
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(It.IsAny<LogContext>(), 1))
            .ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1))
            .ReturnsAsync(new Topic { ID = 1, TopicName = "Classes", SubjectID = 1 });
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<LogContext>(), It.IsAny<Topic>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.UpdateTopic(1, updateDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while updating the topic", statusCodeResult.Value);
    }

    [Fact]
    public async Task DeleteTopic_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<LogContext>(), 1))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.DeleteTopic(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while deleting the topic", statusCodeResult.Value);
    }

    [Fact]
    public async Task GetTopicsBySubject_ReturnsOkResult_WithTopics()
    {
        // Arrange
        var subjectId = 1;
        var topics = new List<Topic>
        {
            new() { ID = 1, TopicName = "T1", SubjectID = subjectId },
            new() { ID = 2, TopicName = "T2", SubjectID = subjectId }
        };
        _mockRepository.Setup(r => r.GetBySubjectIdAsync(It.IsAny<LogContext>(), subjectId)).ReturnsAsync(topics);
        _mapper.Setup(m => m.Map<IEnumerable<TopicDto>>(It.IsAny<IEnumerable<Topic>>()))
            .Returns((IEnumerable<Topic> src) => src.Select(t => new TopicDto { ID = t.ID, TopicName = t.TopicName, SubjectID = t.SubjectID }));

        // Act
        var result = await _controller.GetTopicsBySubject(subjectId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTopics = Assert.IsAssignableFrom<IEnumerable<TopicDto>>(okResult.Value);
        Assert.Equal(2, returnedTopics.Count());
        Assert.All(returnedTopics, t => Assert.Equal(subjectId, t.SubjectID));
    }

    [Fact]
    public async Task GetTopicsBySubject_WhenException_Returns500()
    {
        // Arrange
        var subjectId = 1;
        _mockRepository.Setup(r => r.GetBySubjectIdAsync(It.IsAny<LogContext>(), subjectId)).ThrowsAsync(new System.Exception("fail"));

        // Act
        var result = await _controller.GetTopicsBySubject(subjectId);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
        Assert.Equal("An error occurred while retrieving topics by subject", statusResult.Value);
    }

    [Fact]
    public async Task CreateTopic_WithDuplicateTopic_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateTopicDto { TopicName = "Duplicate Topic", SubjectID = 1 };
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<Topic>(createDto)).Returns(new Topic { TopicName = createDto.TopicName, SubjectID = createDto.SubjectID });
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<LogContext>(), It.IsAny<Topic>())).ThrowsAsync(new DuplicateTopicException(createDto.TopicName, createDto.SubjectID));

        // Act
        var result = await _controller.CreateTopic(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("already exists", badRequestResult.Value.ToString());
    }

    [Fact]
    public async Task UpdateTopic_WithDuplicateTopic_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateTopicDto { TopicName = "Duplicate Topic", SubjectID = 1 };
        _mockSubjectRepository.Setup(repo => repo.SubjectExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(true);
        var existingTopic = new Topic { ID = 1, TopicName = "Old Name", SubjectID = 1 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(existingTopic);
        _mapper.Setup(m => m.Map(updateDto, existingTopic)).Callback(() => { existingTopic.TopicName = updateDto.TopicName; existingTopic.SubjectID = updateDto.SubjectID; });
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<LogContext>(), existingTopic)).ThrowsAsync(new DuplicateTopicException(updateDto.TopicName, updateDto.SubjectID));

        // Act
        var result = await _controller.UpdateTopic(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("already exists", badRequestResult.Value.ToString());
    }
}