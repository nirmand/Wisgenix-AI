using AIUpskillingPlatform.API.Controllers;
using AIUpskillingPlatform.API.DTOs;
using AIUpskillingPlatform.Common.Exceptions;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using AIUpskillingPlatform.Common;
using AIUpskillingPlatform.Core.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            new() { ID = 1, QuestionText = "Test Question 1", TopicID = 1 },
            new() { ID = 2, QuestionText = "Test Question 2", TopicID = 1 }
        };
        _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(questions);

        // Act
        var result = await _controller.GetQuestions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedQuestions = Assert.IsAssignableFrom<IEnumerable<QuestionDto>>(okResult.Value);
        Assert.Equal(2, returnedQuestions.Count());
    }

    [Fact]
    public async Task GetQuestion_WithValidId_ReturnsOkResult_WithQuestion()
    {
        // Arrange
        var question = new Question { ID = 1, QuestionText = "Test Question", TopicID = 1 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(question);

        // Act
        var result = await _controller.GetQuestion(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedQuestion = Assert.IsType<QuestionDto>(okResult.Value);
        Assert.Equal(1, returnedQuestion.ID);
        Assert.Equal("Test Question", returnedQuestion.QuestionText);
    }

    [Fact]
    public async Task GetQuestion_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ThrowsAsync(new QuestionNotFoundException(1));

        // Act
        var result = await _controller.GetQuestion(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Question with ID 1 was not found.", notFoundResult.Value);
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
            GeneratedBy = QuestionSource.AI
        };
        var question = new Question { ID = 1, QuestionText = "New Question", TopicID = 1 };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Question>())).ReturnsAsync(question);

        // Act
        var result = await _controller.CreateQuestion(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedQuestion = Assert.IsType<QuestionDto>(createdResult.Value);
        Assert.Equal(1, returnedQuestion.ID);
        Assert.Equal("New Question", returnedQuestion.QuestionText);
    }

    [Fact]
    public async Task CreateQuestion_WithInvalidTopicId_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateQuestionDto { TopicID = 1 };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(1)).ReturnsAsync(false);

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
            GeneratedBy = QuestionSource.Imported
        };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(new Question { ID = 1 });

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
        _mockRepository.Setup(repo => repo.TopicExistsAsync(1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Question>()))
            .ThrowsAsync(new QuestionNotFoundException(1));

        // Act
        var result = await _controller.UpdateQuestion(1, updateDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Question with ID 1 was not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteQuestion_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteQuestion(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteQuestion_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(1))
            .ThrowsAsync(new QuestionNotFoundException(1));

        // Act
        var result = await _controller.DeleteQuestion(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Question with ID 1 was not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task CreateQuestion_WithValidUrl_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateQuestionDto 
        { 
            QuestionText = "Test Question",
            TopicID = 1,
            DifficultyLevel = 2,
            MaxScore = 5,
            GeneratedBy = QuestionSource.AI,
            QuestionSourceReference = "https://valid-url.com/reference"
        };
        _mockRepository.Setup(repo => repo.TopicExistsAsync(1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Question>()))
            .ReturnsAsync(new Question { ID = 1, QuestionText = createDto.QuestionText });

        // Act
        var result = await _controller.CreateQuestion(createDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedQuestion = Assert.IsType<QuestionDto>(createdAtActionResult.Value);
        Assert.Equal(createDto.QuestionText, returnedQuestion.QuestionText);
    }

    [Fact]
    public async Task CreateQuestion_WithInvalidUrl_ReturnsBadRequest()
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
} 