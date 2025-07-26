using AutoMapper;
using Moq;
using Content.Application.Commands;
using Content.Application.Handlers;
using Content.Application.Mappings;
using Content.Domain.Entities;
using Content.Domain.Repositories;
using Wisgenix.SharedKernel.Application;
using Wisgenix.SharedKernel.Domain.Exceptions;
using Wisgenix.SharedKernel.Domain.Enums;
using Wisgenix.SharedKernel.Infrastructure.Logging;
using Xunit;

namespace Content.Tests.Application;

public class QuestionCommandHandlerTests
{
    private readonly Mock<IQuestionRepository> _mockQuestionRepository;
    private readonly Mock<ITopicRepository> _mockTopicRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILoggingService> _mockLogger;
    private readonly IMapper _mapper;
    private readonly CreateQuestionCommandHandler _createHandler;
    private readonly UpdateQuestionCommandHandler _updateHandler;
    private readonly DeleteQuestionCommandHandler _deleteHandler;

    public QuestionCommandHandlerTests()
    {
        _mockQuestionRepository = new Mock<IQuestionRepository>();
        _mockTopicRepository = new Mock<ITopicRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILoggingService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ContentMappingProfile>());
        _mapper = config.CreateMapper();

        _createHandler = new CreateQuestionCommandHandler(
            _mockQuestionRepository.Object,
            _mockTopicRepository.Object,
            _mockUnitOfWork.Object,
            _mapper,
            _mockLogger.Object);

        _updateHandler = new UpdateQuestionCommandHandler(
            _mockQuestionRepository.Object,
            _mockTopicRepository.Object,
            _mockUnitOfWork.Object,
            _mapper,
            _mockLogger.Object);

        _deleteHandler = new DeleteQuestionCommandHandler(
            _mockQuestionRepository.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CreateQuestionCommandHandler_WithValidData_ShouldCreateQuestion()
    {
        // Arrange
        var command = new CreateQuestionCommand("What is 2 + 2?", 1, 2, 5, QuestionSource.Manual, "https://example.com");
        var question = new Question("What is 2 + 2?", 1, 2, 5, QuestionSource.Manual, "https://example.com");

        _mockTopicRepository.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockQuestionRepository.Setup(x => x.ExistsByTextAndTopicAsync("What is 2 + 2?", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockQuestionRepository.Setup(x => x.AddAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(question);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _createHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("What is 2 + 2?", result.QuestionText);
        Assert.Equal(1, result.TopicId);
        Assert.Equal(2, result.DifficultyLevel);
        Assert.Equal(5, result.MaxScore);
        Assert.Equal(QuestionSource.Manual, result.GeneratedBy);
        _mockQuestionRepository.Verify(x => x.AddAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateQuestionCommandHandler_WithNonExistingTopic_ShouldThrowException()
    {
        // Arrange
        var command = new CreateQuestionCommand("What is 2 + 2?", 999, 2, 5, QuestionSource.Manual);

        _mockTopicRepository.Setup(x => x.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _createHandler.Handle(command, CancellationToken.None));

        _mockQuestionRepository.Verify(x => x.AddAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateQuestionCommandHandler_WithDuplicateText_ShouldThrowException()
    {
        // Arrange
        var command = new CreateQuestionCommand("What is 2 + 2?", 1, 2, 5, QuestionSource.Manual);

        _mockTopicRepository.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockQuestionRepository.Setup(x => x.ExistsByTextAndTopicAsync("What is 2 + 2?", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEntityException>(() => 
            _createHandler.Handle(command, CancellationToken.None));

        _mockQuestionRepository.Verify(x => x.AddAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateQuestionCommandHandler_WithValidData_ShouldUpdateQuestion()
    {
        // Arrange
        var command = new UpdateQuestionCommand(1, "What is 3 + 3?", 1, 3, 7, QuestionSource.AI, "https://updated.com");
        var existingQuestion = new Question("What is 2 + 2?", 1, 2, 5, QuestionSource.Manual);

        _mockQuestionRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingQuestion);
        _mockTopicRepository.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockQuestionRepository.Setup(x => x.ExistsByTextAndTopicAsync("What is 3 + 3?", 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockQuestionRepository.Setup(x => x.UpdateAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingQuestion);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _updateHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("What is 3 + 3?", result.QuestionText);
        _mockQuestionRepository.Verify(x => x.UpdateAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateQuestionCommandHandler_WithNonExistingId_ShouldThrowException()
    {
        // Arrange
        var command = new UpdateQuestionCommand(999, "What is 3 + 3?", 1, 3, 7, QuestionSource.AI);

        _mockQuestionRepository.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Question?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _updateHandler.Handle(command, CancellationToken.None));

        _mockQuestionRepository.Verify(x => x.UpdateAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteQuestionCommandHandler_WithExistingId_ShouldDeleteQuestion()
    {
        // Arrange
        var command = new DeleteQuestionCommand(1);

        _mockQuestionRepository.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _deleteHandler.Handle(command, CancellationToken.None);

        // Assert
        _mockQuestionRepository.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteQuestionCommandHandler_WithNonExistingId_ShouldThrowException()
    {
        // Arrange
        var command = new DeleteQuestionCommand(999);

        _mockQuestionRepository.Setup(x => x.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _deleteHandler.Handle(command, CancellationToken.None));

        _mockQuestionRepository.Verify(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
