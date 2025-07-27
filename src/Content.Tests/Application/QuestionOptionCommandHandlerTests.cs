using Content.Application.Commands;
using Content.Application.Handlers;
using Content.Domain.Entities;
using Content.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Wisgenix.SharedKernel.Application;
using Wisgenix.SharedKernel.Domain.Exceptions;
using Xunit;
using AutoMapper;

namespace Content.Tests.Application;

public class QuestionOptionCommandHandlerTests
{
    private readonly Mock<IQuestionOptionRepository> _mockQuestionOptionRepository;
    private readonly Mock<IQuestionRepository> _mockQuestionRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<CreateQuestionOptionCommandHandler>> _mockCreateLogger;
    private readonly Mock<ILogger<UpdateQuestionOptionCommandHandler>> _mockUpdateLogger;
    private readonly Mock<ILogger<DeleteQuestionOptionCommandHandler>> _mockDeleteLogger;
    private readonly CreateQuestionOptionCommandHandler _createHandler;
    private readonly UpdateQuestionOptionCommandHandler _updateHandler;
    private readonly DeleteQuestionOptionCommandHandler _deleteHandler;

    public QuestionOptionCommandHandlerTests()
    {
        _mockQuestionOptionRepository = new Mock<IQuestionOptionRepository>();
        _mockQuestionRepository = new Mock<IQuestionRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockCreateLogger = new Mock<ILogger<CreateQuestionOptionCommandHandler>>();
        _mockUpdateLogger = new Mock<ILogger<UpdateQuestionOptionCommandHandler>>();
        _mockDeleteLogger = new Mock<ILogger<DeleteQuestionOptionCommandHandler>>();

        _createHandler = new CreateQuestionOptionCommandHandler(
            _mockQuestionOptionRepository.Object,
            _mockQuestionRepository.Object,
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockCreateLogger.Object);

        _updateHandler = new UpdateQuestionOptionCommandHandler(
            _mockQuestionOptionRepository.Object,
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockUpdateLogger.Object);

        _deleteHandler = new DeleteQuestionOptionCommandHandler(
            _mockQuestionOptionRepository.Object,
            _mockUnitOfWork.Object,
            _mockDeleteLogger.Object);
    }

    [Fact]
    public async Task CreateQuestionOptionCommandHandler_WithValidData_ShouldCreateQuestionOption()
    {
        // Arrange
        var command = new CreateQuestionOptionCommand("Correct answer", 1, true);
        var questionOption = new QuestionOption("Correct answer", 1, true);

        _mockQuestionRepository.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockQuestionOptionRepository.Setup(x => x.AddAsync(It.IsAny<QuestionOption>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(questionOption);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockMapper.Setup(x => x.Map<Content.Application.DTOs.GetQuestionOptionResponse>(It.IsAny<QuestionOption>()))
            .Returns(new Content.Application.DTOs.GetQuestionOptionResponse
            {
                Id = questionOption.Id,
                OptionText = questionOption.OptionText,
                QuestionId = questionOption.QuestionId,
                IsCorrect = questionOption.IsCorrect
            });

        // Act
        var result = await _createHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Correct answer", result.OptionText);
        Assert.Equal(1, result.QuestionId);
        Assert.True(result.IsCorrect);
        _mockQuestionOptionRepository.Verify(x => x.AddAsync(It.IsAny<QuestionOption>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateQuestionOptionCommandHandler_WithNonExistingQuestion_ShouldThrowException()
    {
        // Arrange
        var command = new CreateQuestionOptionCommand("Correct answer", 999, true);

        _mockQuestionRepository.Setup(x => x.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _createHandler.Handle(command, CancellationToken.None));

        _mockQuestionOptionRepository.Verify(x => x.AddAsync(It.IsAny<QuestionOption>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateQuestionOptionCommandHandler_WithValidData_ShouldUpdateQuestionOption()
    {
        // Arrange
        var command = new UpdateQuestionOptionCommand(1, "Updated answer", 1, false);
        var existingOption = new QuestionOption("Original answer", 1, true);

        _mockQuestionOptionRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOption);
        _mockQuestionOptionRepository.Setup(x => x.UpdateAsync(It.IsAny<QuestionOption>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOption);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockMapper.Setup(x => x.Map<Content.Application.DTOs.GetQuestionOptionResponse>(It.IsAny<QuestionOption>()))
            .Returns(new Content.Application.DTOs.GetQuestionOptionResponse
            {
                Id = existingOption.Id,
                OptionText = "Updated answer",
                QuestionId = existingOption.QuestionId,
                IsCorrect = false
            });

        // Act
        var result = await _updateHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated answer", result.OptionText);
        Assert.False(result.IsCorrect);
        _mockQuestionOptionRepository.Verify(x => x.UpdateAsync(It.IsAny<QuestionOption>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateQuestionOptionCommandHandler_WithNonExistingId_ShouldThrowException()
    {
        // Arrange
        var command = new UpdateQuestionOptionCommand(999, "Updated answer", 1, false);

        _mockQuestionOptionRepository.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((QuestionOption?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _updateHandler.Handle(command, CancellationToken.None));

        _mockQuestionOptionRepository.Verify(x => x.UpdateAsync(It.IsAny<QuestionOption>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteQuestionOptionCommandHandler_WithExistingId_ShouldDeleteQuestionOption()
    {
        // Arrange
        var command = new DeleteQuestionOptionCommand(1);

        _mockQuestionOptionRepository.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _deleteHandler.Handle(command, CancellationToken.None);

        // Assert
        _mockQuestionOptionRepository.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteQuestionOptionCommandHandler_WithNonExistingId_ShouldThrowException()
    {
        // Arrange
        var command = new DeleteQuestionOptionCommand(999);

        _mockQuestionOptionRepository.Setup(x => x.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _deleteHandler.Handle(command, CancellationToken.None));

        _mockQuestionOptionRepository.Verify(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
