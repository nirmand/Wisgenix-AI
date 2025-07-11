using Wisgenix.API.Controllers;
using Wisgenix.Common;
using Wisgenix.Common.Exceptions;
using Wisgenix.Core.Logger;
using Wisgenix.Data.Entities;
using Wisgenix.DTO;
using Wisgenix.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace Wisgenix.Tests.Controllers;

public class QuestionsControllerTests
{
    private readonly Mock<IQuestionRepository> _mockRepository;
    private readonly Mock<ILoggingService> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly QuestionsController _controller;

    public QuestionsControllerTests()
    {
        _mockRepository = new Mock<IQuestionRepository>();
        _mockLogger = new Mock<ILoggingService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new QuestionsController(_mockRepository.Object, _mockLogger.Object, _mockMapper.Object);

        // Setup ControllerContext with HttpContext and ClaimsPrincipal
        var user = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "testuser")
            }, "mock"));
        var httpContext = new DefaultHttpContext { User = user };
        httpContext.Items["CorrelationId"] = "test-correlation-id";
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    [Fact]
    public async Task GetQuestions_ReturnsOkResult_WithListOfQuestions()
    {
        // Arrange
        var questions = new List<Question>
        {
            new() { ID = 1, QuestionText = "Test Question 1", TopicID = 1, Topic = new Topic { TopicName = "Test Topic" } },
            new() { ID = 2, QuestionText = "Test Question 2", TopicID = 1, Topic = new Topic { TopicName = "Test Topic" } }
        };
        var questionDtos = new List<QuestionDto>
        {
            new() { ID = 1, QuestionText = "Test Question 1", TopicID = 1, TopicName = "Test Topic" },
            new() { ID = 2, QuestionText = "Test Question 2", TopicID = 1, TopicName = "Test Topic" }
        };
        _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<LogContext>())).ReturnsAsync(questions);
        _mockMapper.Setup(m => m.Map<IEnumerable<QuestionDto>>(questions)).Returns(questionDtos);

        // Act
        var result = await _controller.GetQuestions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedQuestions = Assert.IsAssignableFrom<IEnumerable<QuestionDto>>(okResult.Value);
        Assert.Equal(2, returnedQuestions.Count());
        Assert.All(returnedQuestions, q => Assert.Equal("Test Topic", q.TopicName));
    }

    [Fact]
    public async Task GetQuestions_WhenExceptionOccurs_ReturnsInternalServerError()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<LogContext>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetQuestions();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving questions", statusCodeResult.Value);
    }

    [Fact]
    public async Task GetQuestion_WithValidId_ReturnsOkResult_WithQuestion()
    {
        // Arrange
        var question = new Question {
            ID = 1,
            QuestionText = "Test Question",
            TopicID = 1,
            Topic = new Topic { TopicName = "Test Topic" },
            DifficultyLevel = 3,
            MaxScore = 5,
            GeneratedBy = QuestionSource.AI,
            QuestionSourceReference = "https://test.com"
        };
        var questionDto = new QuestionDto {
            ID = 1,
            QuestionText = "Test Question",
            TopicID = 1,
            TopicName = "Test Topic",
            DifficultyLevel = 3,
            MaxScore = 5,
            GeneratedBy = QuestionSource.AI,
            QuestionSourceReference = "https://test.com"
        };
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(question);
        _mockMapper.Setup(m => m.Map<QuestionDto>(question)).Returns(questionDto);

        // Act
        var result = await _controller.GetQuestion(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedQuestion = Assert.IsType<QuestionDto>(okResult.Value);
        Assert.Equal(1, returnedQuestion.ID);
        Assert.Equal("Test Question", returnedQuestion.QuestionText);
        Assert.Equal("Test Topic", returnedQuestion.TopicName);
        Assert.Equal(3, returnedQuestion.DifficultyLevel);
        Assert.Equal(5, returnedQuestion.MaxScore);
        Assert.Equal(QuestionSource.AI, returnedQuestion.GeneratedBy);
        Assert.Equal("https://test.com", returnedQuestion.QuestionSourceReference);
    }

    [Fact]
    public async Task GetQuestion_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1))
            .ThrowsAsync(new QuestionNotFoundException(1));

        // Act
        var result = await _controller.GetQuestion(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Question with ID 1 was not found", notFoundResult.Value);
    }

    [Fact]
    public async Task CreateQuestion_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var userName = "testuser";
        var createDto = new CreateQuestionDto { QuestionText = "What is C#?", TopicID = 1, DifficultyLevel = 1, MaxScore = 5, GeneratedBy = Wisgenix.Common.QuestionSource.User };
        var question = new Question {
            QuestionText = createDto.QuestionText,
            TopicID = createDto.TopicID,
            DifficultyLevel = createDto.DifficultyLevel,
            MaxScore = createDto.MaxScore,
            GeneratedBy = createDto.GeneratedBy,
            QuestionSourceReference = createDto.QuestionSourceReference
        };
        var createdQuestion = new Question {
            ID = 1,
            QuestionText = "What is C#?",
            TopicID = 1,
            DifficultyLevel = 1,
            MaxScore = 5,
            GeneratedBy = Wisgenix.Common.QuestionSource.User,
            CreatedDate = now,
            CreatedBy = userName,
            ModifiedDate = null,
            ModifiedBy = null
        };
        var questionDto = new QuestionDto {
            ID = 1,
            QuestionText = "What is C#?",
            TopicID = 1,
            DifficultyLevel = 1,
            MaxScore = 5,
            GeneratedBy = Wisgenix.Common.QuestionSource.User,
            CreatedDate = now,
            CreatedBy = userName,
            ModifiedDate = null,
            ModifiedBy = null
        };
        _mockMapper.Setup(m => m.Map<Question>(createDto)).Returns(question);
        _mockMapper.Setup(m => m.Map<QuestionDto>(createdQuestion)).Returns(questionDto);
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<LogContext>(), question)).ReturnsAsync(createdQuestion);
        _mockRepository.Setup(repo => repo.TopicExistsAsync(It.IsAny<LogContext>(), createDto.TopicID)).ReturnsAsync(true);

        // Act
        var result = await _controller.CreateQuestion(createDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedQuestion = Assert.IsType<QuestionDto>(createdAtActionResult.Value);
        Assert.Equal(createDto.QuestionText, returnedQuestion.QuestionText);
        Assert.Equal(now, returnedQuestion.CreatedDate);
        Assert.Equal(userName, returnedQuestion.CreatedBy);
        Assert.Null(returnedQuestion.ModifiedDate);
        Assert.Null(returnedQuestion.ModifiedBy);
    }

    [Fact]
    public async Task CreateQuestion_WithInvalidTopicId_ReturnsNotFound()
    {
        // Arrange
        var createDto = new CreateQuestionDto { TopicID = 1 };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(false);

        // Act
        var result = await _controller.CreateQuestion(createDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal($"Topic with ID {createDto.TopicID} was not found", notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateQuestion_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var userName = "testuser";
        var updateDto = new UpdateQuestionDto { QuestionText = "Updated Q?", TopicID = 1, DifficultyLevel = 2, MaxScore = 10, GeneratedBy = Wisgenix.Common.QuestionSource.User };
        var question = new Question {
            ID = 1,
            QuestionText = updateDto.QuestionText,
            TopicID = updateDto.TopicID,
            DifficultyLevel = updateDto.DifficultyLevel,
            MaxScore = updateDto.MaxScore,
            GeneratedBy = updateDto.GeneratedBy,
            QuestionSourceReference = updateDto.QuestionSourceReference
        };
        var existingQuestion = new Question {
            ID = 1,
            QuestionText = "What is C#?",
            TopicID = 1,
            DifficultyLevel = 1,
            MaxScore = 5,
            GeneratedBy = Wisgenix.Common.QuestionSource.User,
            CreatedDate = now.AddDays(-1),
            CreatedBy = userName,
            ModifiedDate = null,
            ModifiedBy = null
        };
        var updatedQuestion = new Question {
            ID = 1,
            QuestionText = "Updated Q?",
            TopicID = 1,
            DifficultyLevel = 2,
            MaxScore = 10,
            GeneratedBy = Wisgenix.Common.QuestionSource.User,
            CreatedDate = now.AddDays(-1),
            CreatedBy = userName,
            ModifiedDate = now,
            ModifiedBy = userName
        };
        _mockMapper.Setup(m => m.Map<Question>(updateDto)).Returns(question);
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(existingQuestion);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<LogContext>(), question)).ReturnsAsync(updatedQuestion);
        _mockRepository.Setup(repo => repo.TopicExistsAsync(It.IsAny<LogContext>(), updateDto.TopicID)).ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateQuestion(1, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<LogContext>(), question), Times.Once);
        Assert.Equal(now, updatedQuestion.ModifiedDate);
        Assert.Equal(userName, updatedQuestion.ModifiedBy);
    }

    [Fact]
    public async Task UpdateQuestion_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateQuestionDto { TopicID = 1 };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<LogContext>(), It.IsAny<Question>()))
            .ThrowsAsync(new QuestionNotFoundException(1));
        _mockMapper.Setup(m => m.Map<Question>(updateDto)).Returns(new Question { ID = 1, TopicID = 1 });

        // Act
        var result = await _controller.UpdateQuestion(1, updateDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Question with ID 1 was not found", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteQuestion_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<LogContext>(), 1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteQuestion(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteQuestion_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<LogContext>(), 1))
            .ThrowsAsync(new QuestionNotFoundException(1));

        // Act
        var result = await _controller.DeleteQuestion(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Question with ID 1 was not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetQuestion_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetQuestion(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving the question", statusCodeResult.Value);
    }

    [Fact]
    public async Task CreateQuestion_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        var createDto = new CreateQuestionDto
        {
            QuestionText = "Test Question",
            TopicID = 1,
            DifficultyLevel = 2,
            MaxScore = 5,
            GeneratedBy = QuestionSource.AI,
            QuestionSourceReference = "https://test.com"
        };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<LogContext>(), It.IsAny<Question>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.CreateQuestion(createDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while creating the question", statusCodeResult.Value);
    }

    [Fact]
    public async Task UpdateQuestion_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        var updateDto = new UpdateQuestionDto
        {
            QuestionText = "Updated Question",
            TopicID = 1,
            DifficultyLevel = 2,
            MaxScore = 5,
            GeneratedBy = QuestionSource.AI,
            QuestionSourceReference = "https://test.com"
        };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<LogContext>(), It.IsAny<Question>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.UpdateQuestion(1, updateDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while updating the question", statusCodeResult.Value);
    }

    [Fact]
    public async Task DeleteQuestion_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<LogContext>(), 1))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.DeleteQuestion(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while deleting the question", statusCodeResult.Value);
    }

    [Fact]
    public async Task GetQuestionsByTopic_ReturnsOkResult_WithQuestions()
    {
        // Arrange
        var topicId = 1;
        var questions = new List<Question>
        {
            new() { ID = 1, QuestionText = "Q1", TopicID = topicId, Topic = new Topic { TopicName = "Topic1" } },
            new() { ID = 2, QuestionText = "Q2", TopicID = topicId, Topic = new Topic { TopicName = "Topic1" } }
        };
        _mockRepository.Setup(r => r.GetByTopicIdAsync(It.IsAny<LogContext>(), topicId)).ReturnsAsync(questions);
        _mockMapper.Setup(m => m.Map<IEnumerable<QuestionDto>>(questions)).Returns(questions.Select(q => new QuestionDto
        {
            ID = q.ID,
            QuestionText = q.QuestionText,
            TopicID = q.TopicID,
            TopicName = q.Topic.TopicName
        }));

        // Act
        var result = await _controller.GetQuestionsByTopic(topicId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedQuestions = Assert.IsAssignableFrom<IEnumerable<QuestionDto>>(okResult.Value);
        Assert.Equal(2, returnedQuestions.Count());
        Assert.All(returnedQuestions, q => Assert.Equal(topicId, q.TopicID));
    }

    [Fact]
    public async Task GetQuestionsByTopic_WhenException_Returns500()
    {
        // Arrange
        var topicId = 1;
        _mockRepository.Setup(r => r.GetByTopicIdAsync(It.IsAny<LogContext>(), topicId)).ThrowsAsync(new System.Exception("fail"));

        // Act
        var result = await _controller.GetQuestionsByTopic(topicId);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
        Assert.Equal("An error occurred while retrieving questions by topic", statusResult.Value);
    }

    [Fact]
    public async Task CreateQuestion_WithDuplicateQuestion_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateQuestionDto
        {
            QuestionText = "Duplicate Question",
            TopicID = 1,
            DifficultyLevel = 2,
            MaxScore = 1,
            GeneratedBy = QuestionSource.AI,
            QuestionSourceReference = "https://test.com"
        };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(true);
        _mockMapper.Setup(m => m.Map<Question>(createDto)).Returns(new Question { QuestionText = createDto.QuestionText, TopicID = createDto.TopicID });
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<LogContext>(), It.IsAny<Question>())).ThrowsAsync(new DuplicateQuestionException(createDto.QuestionText, createDto.TopicID));

        // Act
        var result = await _controller.CreateQuestion(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("already exists", badRequestResult.Value.ToString());
    }

    [Fact]
    public async Task UpdateQuestion_WithDuplicateQuestion_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateQuestionDto
        {
            QuestionText = "Duplicate Question",
            TopicID = 1,
            DifficultyLevel = 2,
            MaxScore = 1,
            GeneratedBy = QuestionSource.AI,
            QuestionSourceReference = "https://test.com"
        };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(true);
        _mockMapper.Setup(m => m.Map<Question>(updateDto)).Returns(new Question { QuestionText = updateDto.QuestionText, TopicID = updateDto.TopicID });
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<LogContext>(), It.IsAny<Question>())).ThrowsAsync(new DuplicateQuestionException(updateDto.QuestionText, updateDto.TopicID));

        // Act
        var result = await _controller.UpdateQuestion(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("already exists", badRequestResult.Value.ToString());
    }
}