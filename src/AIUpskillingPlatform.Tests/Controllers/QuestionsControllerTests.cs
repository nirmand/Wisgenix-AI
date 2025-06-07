using AIUpskillingPlatform.API.Controllers;
using AIUpskillingPlatform.Common;
using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Core.Logger;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.DTO;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AIUpskillingPlatform.Tests.Controllers;

public class QuestionsControllerTests
{
    private readonly Mock<IQuestionRepository> _mockRepository;
    private readonly Mock<ILoggingService> _mockLogger;
    private readonly QuestionsController _controller;

    public QuestionsControllerTests()
    {
        _mockRepository = new Mock<IQuestionRepository>();
        _mockLogger = new Mock<ILoggingService>();
        _controller = new QuestionsController(_mockRepository.Object, _mockLogger.Object);
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
        _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<LogContext>())).ReturnsAsync(questions);

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
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(question);

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
        var createDto = new CreateQuestionDto
        {
            QuestionText = "New Question",
            TopicID = 1,
            DifficultyLevel = 1,
            MaxScore = 10,
            GeneratedBy = QuestionSource.AI,
            QuestionSourceReference = "https://test.com"
        };
        var question = new Question { 
            ID = 1, 
            QuestionText = createDto.QuestionText, 
            TopicID = createDto.TopicID,
            DifficultyLevel = createDto.DifficultyLevel,
            MaxScore = createDto.MaxScore,
            GeneratedBy = createDto.GeneratedBy,
            QuestionSourceReference = createDto.QuestionSourceReference
        };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<LogContext>(), It.IsAny<Question>())).ReturnsAsync(question);

        // Act
        var result = await _controller.CreateQuestion(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedQuestion = Assert.IsType<QuestionDto>(createdResult.Value);
        Assert.Equal(1, returnedQuestion.ID);
        Assert.Equal("New Question", returnedQuestion.QuestionText);
        Assert.Equal(10, returnedQuestion.MaxScore);
        Assert.Equal(QuestionSource.AI, returnedQuestion.GeneratedBy);
        Assert.Equal("https://test.com", returnedQuestion.QuestionSourceReference);
    }

    [Fact]
    public async Task CreateQuestion_WithInvalidTopicId_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateQuestionDto { TopicID = 1 };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(false);

        // Act
        var result = await _controller.CreateQuestion(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Topic with ID 1 does not exist", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateQuestion_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var updateDto = new UpdateQuestionDto
        {
            QuestionText = "Updated Question",
            TopicID = 1,
            DifficultyLevel = 2,
            MaxScore = 15,
            GeneratedBy = QuestionSource.AI,
            QuestionSourceReference = "https://test.com"
        };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<LogContext>(), It.IsAny<Question>())).ReturnsAsync(new Question());

        // Act
        var result = await _controller.UpdateQuestion(1, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateQuestion_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateQuestionDto { TopicID = 1 };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<LogContext>(), It.IsAny<Question>()))
            .ThrowsAsync(new QuestionNotFoundException(1));

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
    public async Task CreateQuestion_WithInvalidValidationUrl_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateQuestionDto 
        { 
            QuestionText = "Test Question",
            TopicID = 1,
            DifficultyLevel = 2,
            MaxScore = 5,
            GeneratedBy = QuestionSource.AI,
            QuestionSourceReference = "invalid-url"
        };
        _controller.ModelState.AddModelError("QuestionSourceReference", "Invalid URL format");

        // Act
        var result = await _controller.CreateQuestion(createDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
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
    public async Task UpdateQuestion_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateQuestionDto();
        _controller.ModelState.AddModelError("QuestionText", "Question text is required");

        // Act
        var result = await _controller.UpdateQuestion(1, updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateQuestion_WithInvalidMaxScore_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateQuestionDto 
        { 
            QuestionText = "Test Question",
            TopicID = 1,
            DifficultyLevel = 2,
            MaxScore = 0, // Invalid score
            GeneratedBy = QuestionSource.AI,
            QuestionSourceReference = "https://test.com"
        };
        _controller.ModelState.AddModelError("MaxScore", "Max score must be greater than 0");

        // Act
        var result = await _controller.CreateQuestion(createDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateQuestion_WithInvalidTopicId_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateQuestionDto
        {
            QuestionText = "Updated Question",
            TopicID = 1
        };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(It.IsAny<LogContext>(), 1))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateQuestion(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Topic with ID 1 does not exist", badRequestResult.Value);
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
}