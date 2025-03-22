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

public class QuestionOptionsControllerTests
{
    private readonly Mock<IQuestionOptionRepository> _mockRepository;
    private readonly Mock<ILogger<QuestionOptionsController>> _mockLogger;
    private readonly QuestionOptionsController _controller;

    public QuestionOptionsControllerTests()
    {
        _mockRepository = new Mock<IQuestionOptionRepository>();
        _mockLogger = new Mock<ILogger<QuestionOptionsController>>();
        _controller = new QuestionOptionsController(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetQuestionOptions_ReturnsOkResult_WithListOfOptions()
    {
        // Arrange
        var options = new List<QuestionOption>
        {
            new() { ID = 1, OptionText = "Option 1", IsCorrect = true, QuestionID = 1 },
            new() { ID = 2, OptionText = "Option 2", IsCorrect = false, QuestionID = 1 }
        };
        _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(options);

        // Act
        var result = await _controller.GetQuestionOptions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOptions = Assert.IsAssignableFrom<IEnumerable<QuestionOptionDto>>(okResult.Value);
        Assert.Equal(2, returnedOptions.Count());
    }

    [Fact]
    public async Task GetQuestionOption_WithValidId_ReturnsOkResult_WithOption()
    {
        // Arrange
        var option = new QuestionOption { ID = 1, OptionText = "Test Option", IsCorrect = true, QuestionID = 1 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(option);

        // Act
        var result = await _controller.GetQuestionOption(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOption = Assert.IsType<QuestionOptionDto>(okResult.Value);
        Assert.Equal(1, returnedOption.ID);
        Assert.Equal("Test Option", returnedOption.OptionText);
        Assert.True(returnedOption.IsCorrect);
    }

    [Fact]
    public async Task GetQuestionOption_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ThrowsAsync(new QuestionOptionNotFoundException(1));

        // Act
        var result = await _controller.GetQuestionOption(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Question option with ID 1 was not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetOptionsByQuestion_ReturnsOkResult_WithListOfOptions()
    {
        // Arrange
        var options = new List<QuestionOption>
        {
            new() { ID = 1, OptionText = "Option 1", IsCorrect = true, QuestionID = 1 },
            new() { ID = 2, OptionText = "Option 2", IsCorrect = false, QuestionID = 1 }
        };
        _mockRepository.Setup(repo => repo.GetByQuestionIdAsync(1)).ReturnsAsync(options);

        // Act
        var result = await _controller.GetOptionsByQuestion(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOptions = Assert.IsAssignableFrom<IEnumerable<QuestionOptionDto>>(okResult.Value);
        Assert.Equal(2, returnedOptions.Count());
    }

    [Fact]
    public async Task CreateQuestionOption_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateQuestionOptionDto
        {
            OptionText = "New Option",
            IsCorrect = true,
            QuestionID = 1
        };
        var option = new QuestionOption { ID = 1, OptionText = "New Option", IsCorrect = true, QuestionID = 1 };
        _mockRepository.Setup(repo => repo.QuestionExistsAsync(1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<QuestionOption>())).ReturnsAsync(option);

        // Act
        var result = await _controller.CreateQuestionOption(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedOption = Assert.IsType<QuestionOptionDto>(createdResult.Value);
        Assert.Equal(1, returnedOption.ID);
        Assert.Equal("New Option", returnedOption.OptionText);
        Assert.True(returnedOption.IsCorrect);
    }

    [Fact]
    public async Task CreateQuestionOption_WithInvalidQuestionId_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateQuestionOptionDto { QuestionID = 1 };
        _mockRepository.Setup(repo => repo.QuestionExistsAsync(1)).ReturnsAsync(false);

        // Act
        var result = await _controller.CreateQuestionOption(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Question with ID 1 does not exist", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateQuestionOption_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var updateDto = new UpdateQuestionOptionDto
        {
            OptionText = "Updated Option",
            IsCorrect = false,
            QuestionID = 1
        };
        _mockRepository.Setup(repo => repo.QuestionExistsAsync(1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<QuestionOption>())).Returns((Task<QuestionOption>)Task.CompletedTask);

        // Act
        var result = await _controller.UpdateQuestionOption(1, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateQuestionOption_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateQuestionOptionDto { QuestionID = 1 };
        _mockRepository.Setup(repo => repo.QuestionExistsAsync(1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<QuestionOption>()))
            .ThrowsAsync(new QuestionOptionNotFoundException(1));

        // Act
        var result = await _controller.UpdateQuestionOption(1, updateDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Question option with ID 1 was not found", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteQuestionOption_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteQuestionOption(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteQuestionOption_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(1))
            .ThrowsAsync(new QuestionOptionNotFoundException(1));

        // Act
        var result = await _controller.DeleteQuestionOption(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Question option with ID 1 was not found", notFoundResult.Value);
    }
} 