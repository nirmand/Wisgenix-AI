using Wisgenix.API.Controllers;
using Wisgenix.Common.Exceptions;
using Wisgenix.Core.Logger;
using Wisgenix.Data.Entities;
using Wisgenix.DTO;
using Wisgenix.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Wisgenix.Tests.Controllers;

public class QuestionOptionsControllerTests
{
    private readonly Mock<IQuestionOptionRepository> _mockRepository;
    private readonly Mock<ILoggingService> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly QuestionOptionsController _controller;

    public QuestionOptionsControllerTests()
    {
        _mockRepository = new Mock<IQuestionOptionRepository>();
        _mockLogger = new Mock<ILoggingService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new QuestionOptionsController(_mockRepository.Object, _mockLogger.Object, _mockMapper.Object);

        _mockMapper.Setup(m => m.Map<QuestionOptionDto>(It.IsAny<QuestionOption>()))
            .Returns((QuestionOption source) => new QuestionOptionDto
            {
                ID = source.ID,
                OptionText = source.OptionText,
                IsCorrect = source.IsCorrect,
                QuestionID = source.QuestionID,
                QuestionText = source.Question?.QuestionText ?? string.Empty
            });

        _mockMapper.Setup(m => m.Map<IEnumerable<QuestionOptionDto>>(It.IsAny<IEnumerable<QuestionOption>>()))
            .Returns((IEnumerable<QuestionOption> source) => source.Select(o => new QuestionOptionDto
            {
                ID = o.ID,
                OptionText = o.OptionText,
                IsCorrect = o.IsCorrect,
                QuestionID = o.QuestionID,
                QuestionText = o.Question?.QuestionText ?? string.Empty
            }));

        _mockMapper.Setup(m => m.Map<QuestionOption>(It.IsAny<CreateQuestionOptionDto>()))
            .Returns((CreateQuestionOptionDto source) => new QuestionOption
            {
                OptionText = source.OptionText,
                IsCorrect = source.IsCorrect,
                QuestionID = source.QuestionID
            });

        _mockMapper.Setup(m => m.Map<QuestionOption>(It.IsAny<UpdateQuestionOptionDto>()))
            .Returns((UpdateQuestionOptionDto source) => new QuestionOption
            {
                OptionText = source.OptionText,
                IsCorrect = source.IsCorrect,
                QuestionID = source.QuestionID
            });
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
        _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<LogContext>())).ReturnsAsync(options);

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
        var option = new QuestionOption
        {
            ID = 1,
            OptionText = "Test Option",
            IsCorrect = true,
            QuestionID = 1,
            Question = new Question { QuestionText = "Test Question" }
        };
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(option);

        // Act
        var result = await _controller.GetQuestionOption(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOption = Assert.IsType<QuestionOptionDto>(okResult.Value);
        Assert.Equal(1, returnedOption.ID);
        Assert.Equal("Test Option", returnedOption.OptionText);
        Assert.True(returnedOption.IsCorrect);
        Assert.Equal("Test Question", returnedOption.QuestionText);
    }

    [Fact]
    public async Task GetQuestionOption_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1))
            .ThrowsAsync(new QuestionOptionNotFoundException(1));

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
        _mockRepository.Setup(repo => repo.GetByQuestionIdAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(options);

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
        var createdOption = new QuestionOption
        {
            ID = 1,
            OptionText = "New Option",
            IsCorrect = true,
            QuestionID = 1,
            Question = new Question { QuestionText = "Test Question" }
        };
        _mockRepository.Setup(repo => repo.QuestionExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<LogContext>(), It.IsAny<QuestionOption>()))
            .ReturnsAsync(createdOption);

        // Act
        var result = await _controller.CreateQuestionOption(createDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedOption = Assert.IsType<QuestionOptionDto>(createdAtActionResult.Value);
        Assert.Equal(1, returnedOption.ID);
        Assert.Equal("New Option", returnedOption.OptionText);
        Assert.True(returnedOption.IsCorrect);
        Assert.Equal("Test Question", returnedOption.QuestionText);
    }

    [Fact]
    public async Task CreateQuestionOption_WithInvalidQuestionId_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateQuestionOptionDto { QuestionID = 1 };
        _mockRepository.Setup(repo => repo.QuestionExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(false);

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
        _mockRepository.Setup(repo => repo.QuestionExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<LogContext>(), It.IsAny<QuestionOption>()))
            .ReturnsAsync(new QuestionOption());

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
        _mockRepository.Setup(repo => repo.QuestionExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<LogContext>(), It.IsAny<QuestionOption>()))
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
        _mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<LogContext>(), 1))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteQuestionOption(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteQuestionOption_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<LogContext>(), 1))
            .ThrowsAsync(new QuestionOptionNotFoundException(1));

        // Act
        var result = await _controller.DeleteQuestionOption(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Question option with ID 1 was not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetQuestionOptions_WhenExceptionOccurs_ReturnsInternalServerError()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<LogContext>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetQuestionOptions();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving question options", statusCodeResult.Value);
    }

    [Fact]
    public async Task GetOptionsByQuestion_WhenExceptionOccurs_ReturnsInternalServerError()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByQuestionIdAsync(It.IsAny<LogContext>(), 1))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetOptionsByQuestion(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving question options", statusCodeResult.Value);
    }

    [Fact]
    public async Task CreateQuestionOption_WhenDatabaseUpdateFails_ReturnsInternalServerError()
    {
        // Arrange
        var createDto = new CreateQuestionOptionDto
        {
            OptionText = "New Option",
            IsCorrect = true,
            QuestionID = 1
        };
        _mockRepository.Setup(repo => repo.QuestionExistsAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<LogContext>(), It.IsAny<QuestionOption>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateQuestionOption(createDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while creating the question option", statusCodeResult.Value);
    }

    [Fact]
    public async Task CreateQuestionOption_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateQuestionOptionDto();
        _controller.ModelState.AddModelError("OptionText", "Option text is required");

        // Act
        var result = await _controller.CreateQuestionOption(createDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateQuestionOption_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateQuestionOptionDto();
        _controller.ModelState.AddModelError("OptionText", "Option text is required");

        // Act
        var result = await _controller.UpdateQuestionOption(1, updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}