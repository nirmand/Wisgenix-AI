using AutoMapper;
using Moq;
using Content.Application.Commands;
using Content.Application.Handlers;
using Content.Application.Mappings;
using Content.Domain.Entities;
using Content.Domain.Repositories;
using Wisgenix.SharedKernel.Application;
using Wisgenix.SharedKernel.Domain.Exceptions;
using Wisgenix.SharedKernel.Infrastructure.Logging;
using Xunit;

namespace Content.Tests.Application;

public class TopicCommandHandlerTests
{
    private readonly Mock<ITopicRepository> _mockTopicRepository;
    private readonly Mock<ISubjectRepository> _mockSubjectRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILoggingService> _mockLogger;
    private readonly IMapper _mapper;
    private readonly CreateTopicCommandHandler _createHandler;
    private readonly UpdateTopicCommandHandler _updateHandler;
    private readonly DeleteTopicCommandHandler _deleteHandler;

    public TopicCommandHandlerTests()
    {
        _mockTopicRepository = new Mock<ITopicRepository>();
        _mockSubjectRepository = new Mock<ISubjectRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILoggingService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ContentMappingProfile>());
        _mapper = config.CreateMapper();

        _createHandler = new CreateTopicCommandHandler(
            _mockTopicRepository.Object,
            _mockSubjectRepository.Object,
            _mockUnitOfWork.Object,
            _mapper,
            _mockLogger.Object);

        _updateHandler = new UpdateTopicCommandHandler(
            _mockTopicRepository.Object,
            _mockSubjectRepository.Object,
            _mockUnitOfWork.Object,
            _mapper,
            _mockLogger.Object);

        _deleteHandler = new DeleteTopicCommandHandler(
            _mockTopicRepository.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CreateTopicCommandHandler_WithValidData_ShouldCreateTopic()
    {
        // Arrange
        var command = new CreateTopicCommand("Algebra", 1);
        var topic = new Topic("Algebra", 1);

        _mockSubjectRepository.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockTopicRepository.Setup(x => x.ExistsByNameAndSubjectAsync("Algebra", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockTopicRepository.Setup(x => x.AddAsync(It.IsAny<Topic>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(topic);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _createHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Algebra", result.TopicName);
        Assert.Equal(1, result.SubjectId);
        _mockTopicRepository.Verify(x => x.AddAsync(It.IsAny<Topic>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTopicCommandHandler_WithNonExistingSubject_ShouldThrowException()
    {
        // Arrange
        var command = new CreateTopicCommand("Algebra", 999);

        _mockSubjectRepository.Setup(x => x.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _createHandler.Handle(command, CancellationToken.None));

        _mockTopicRepository.Verify(x => x.AddAsync(It.IsAny<Topic>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateTopicCommandHandler_WithDuplicateName_ShouldThrowException()
    {
        // Arrange
        var command = new CreateTopicCommand("Algebra", 1);

        _mockSubjectRepository.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockTopicRepository.Setup(x => x.ExistsByNameAndSubjectAsync("Algebra", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEntityException>(() => 
            _createHandler.Handle(command, CancellationToken.None));

        _mockTopicRepository.Verify(x => x.AddAsync(It.IsAny<Topic>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTopicCommandHandler_WithValidData_ShouldUpdateTopic()
    {
        // Arrange
        var command = new UpdateTopicCommand(1, "Advanced Algebra", 1);
        var existingTopic = new Topic("Algebra", 1);

        _mockTopicRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTopic);
        _mockSubjectRepository.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockTopicRepository.Setup(x => x.ExistsByNameAndSubjectAsync("Advanced Algebra", 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockTopicRepository.Setup(x => x.UpdateAsync(It.IsAny<Topic>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTopic);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _updateHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Advanced Algebra", result.TopicName);
        _mockTopicRepository.Verify(x => x.UpdateAsync(It.IsAny<Topic>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTopicCommandHandler_WithNonExistingId_ShouldThrowException()
    {
        // Arrange
        var command = new UpdateTopicCommand(999, "Advanced Algebra", 1);

        _mockTopicRepository.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Topic?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _updateHandler.Handle(command, CancellationToken.None));

        _mockTopicRepository.Verify(x => x.UpdateAsync(It.IsAny<Topic>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTopicCommandHandler_WithExistingId_ShouldDeleteTopic()
    {
        // Arrange
        var command = new DeleteTopicCommand(1);

        _mockTopicRepository.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _deleteHandler.Handle(command, CancellationToken.None);

        // Assert
        _mockTopicRepository.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTopicCommandHandler_WithNonExistingId_ShouldThrowException()
    {
        // Arrange
        var command = new DeleteTopicCommand(999);

        _mockTopicRepository.Setup(x => x.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _deleteHandler.Handle(command, CancellationToken.None));

        _mockTopicRepository.Verify(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
