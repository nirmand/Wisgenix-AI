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

public class SubjectCommandHandlerTests
{
    private readonly Mock<ISubjectRepository> _mockSubjectRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILoggingService> _mockLogger;
    private readonly IMapper _mapper;
    private readonly CreateSubjectCommandHandler _createHandler;
    private readonly UpdateSubjectCommandHandler _updateHandler;
    private readonly DeleteSubjectCommandHandler _deleteHandler;

    public SubjectCommandHandlerTests()
    {
        _mockSubjectRepository = new Mock<ISubjectRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILoggingService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ContentMappingProfile>());
        _mapper = config.CreateMapper();

        _createHandler = new CreateSubjectCommandHandler(
            _mockSubjectRepository.Object,
            _mockUnitOfWork.Object,
            _mapper,
            _mockLogger.Object);

        _updateHandler = new UpdateSubjectCommandHandler(
            _mockSubjectRepository.Object,
            _mockUnitOfWork.Object,
            _mapper,
            _mockLogger.Object);

        _deleteHandler = new DeleteSubjectCommandHandler(
            _mockSubjectRepository.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CreateSubjectCommandHandler_WithValidData_ShouldCreateSubject()
    {
        // Arrange
        var command = new CreateSubjectCommand("Mathematics");
        var subject = new Subject("Mathematics");

        _mockSubjectRepository.Setup(x => x.ExistsByNameAsync("Mathematics", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockSubjectRepository.Setup(x => x.AddAsync(It.IsAny<Subject>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(subject);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _createHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Mathematics", result.SubjectName);
        _mockSubjectRepository.Verify(x => x.AddAsync(It.IsAny<Subject>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateSubjectCommandHandler_WithDuplicateName_ShouldThrowException()
    {
        // Arrange
        var command = new CreateSubjectCommand("Mathematics");

        _mockSubjectRepository.Setup(x => x.ExistsByNameAsync("Mathematics", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEntityException>(() => 
            _createHandler.Handle(command, CancellationToken.None));

        _mockSubjectRepository.Verify(x => x.AddAsync(It.IsAny<Subject>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateSubjectCommandHandler_WithValidData_ShouldUpdateSubject()
    {
        // Arrange
        var command = new UpdateSubjectCommand(1, "Advanced Mathematics");
        var existingSubject = new Subject("Mathematics");

        _mockSubjectRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSubject);
        _mockSubjectRepository.Setup(x => x.ExistsByNameAsync("Advanced Mathematics", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockSubjectRepository.Setup(x => x.UpdateAsync(It.IsAny<Subject>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSubject);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _updateHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Advanced Mathematics", result.SubjectName);
        _mockSubjectRepository.Verify(x => x.UpdateAsync(It.IsAny<Subject>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateSubjectCommandHandler_WithNonExistingId_ShouldThrowException()
    {
        // Arrange
        var command = new UpdateSubjectCommand(999, "Advanced Mathematics");

        _mockSubjectRepository.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subject?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _updateHandler.Handle(command, CancellationToken.None));

        _mockSubjectRepository.Verify(x => x.UpdateAsync(It.IsAny<Subject>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteSubjectCommandHandler_WithExistingId_ShouldDeleteSubject()
    {
        // Arrange
        var command = new DeleteSubjectCommand(1);

        _mockSubjectRepository.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _deleteHandler.Handle(command, CancellationToken.None);

        // Assert
        _mockSubjectRepository.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteSubjectCommandHandler_WithNonExistingId_ShouldThrowException()
    {
        // Arrange
        var command = new DeleteSubjectCommand(999);

        _mockSubjectRepository.Setup(x => x.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _deleteHandler.Handle(command, CancellationToken.None));

        _mockSubjectRepository.Verify(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
