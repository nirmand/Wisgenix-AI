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

namespace Wisgenix.Tests.Controllers;

public class SubjectsControllerTests
{
    private readonly Mock<ISubjectRepository> _mockRepository;
    private readonly Mock<ILoggingService> _mockLogger;
    private readonly SubjectsController _controller;
    private readonly Mock<IMapper> _mapper;

    public SubjectsControllerTests()
    {
        _mockRepository = new Mock<ISubjectRepository>();
        _mockLogger = new Mock<ILoggingService>();
        _mapper = new Mock<IMapper>();
        _controller = new SubjectsController(_mockRepository.Object, _mockLogger.Object, _mapper.Object);
        
        _mapper.Setup(m => m.Map<SubjectDto>(It.IsAny<Subject>()))
            .Returns((Subject source) => new SubjectDto
            {
                ID = source.ID,
                SubjectName = source.SubjectName,
                Topics = source.Topics?.Select(t => new TopicDto { ID = t.ID, TopicName = t.TopicName, SubjectID = t.SubjectID }).ToList() ?? new List<TopicDto>(),
                CreatedDate = source.CreatedDate,
                CreatedBy = source.CreatedBy,
                ModifiedDate = source.ModifiedDate,
                ModifiedBy = source.ModifiedBy
            });
        
        _mapper.Setup(m => m.Map<IEnumerable<SubjectDto>>(It.IsAny<IEnumerable<Subject>>()))
            .Returns((IEnumerable<Subject> source) => source.Select(s => new SubjectDto 
            { 
                ID = s.ID, 
                SubjectName = s.SubjectName 
            }));

        _mapper.Setup(m => m.Map<Subject>(It.IsAny<CreateSubjectDto>()))
            .Returns((CreateSubjectDto source) => new Subject
            {
                SubjectName = source.SubjectName,
                Topics = new List<Topic>()
            });
    }

    [Fact]
    public async Task GetSubjects_ReturnsOkResult_WithSubjects()
    {
        // Arrange
        var subjects = new List<Subject>
        {
            new Subject { ID = 1, SubjectName = "C#", Topics = new List<Topic>() },
            new Subject { ID = 2, SubjectName = "Java", Topics = new List<Topic>() }
        };
        _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<LogContext>())).ReturnsAsync(subjects);

        // Act
        var result = await _controller.GetSubjects();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedSubjects = Assert.IsAssignableFrom<IEnumerable<SubjectDto>>(okResult.Value);
        Assert.Equal(2, returnedSubjects.Count());
    }

    [Fact]
    public async Task GetSubject_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var subject = new Subject { ID = 1, SubjectName = "C#", Topics = new List<Topic>() };
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(),1)).ReturnsAsync(subject);

        // Act
        var result = await _controller.GetSubject(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedSubject = Assert.IsType<SubjectDto>(okResult.Value);
        Assert.Equal(subject.SubjectName, returnedSubject.SubjectName);
    }

    [Fact]
    public async Task GetSubject_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        int invalidId = 999;
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(),invalidId)).ThrowsAsync(new SubjectNotFoundException(invalidId));

        // Act
        var result = await _controller.GetSubject(invalidId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateSubject_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var userName = "testuser";
        var createDto = new CreateSubjectDto { SubjectName = "C#" };
        var createdSubject = new Subject {
            ID = 1,
            SubjectName = "C#",
            CreatedDate = now,
            CreatedBy = userName,
            ModifiedDate = null,
            ModifiedBy = null,
            Topics = new List<Topic>()
        };
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<LogContext>(), It.IsAny<Subject>())).ReturnsAsync(createdSubject);

        // Act
        var result = await _controller.CreateSubject(createDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedSubject = Assert.IsType<SubjectDto>(createdAtActionResult.Value);
        Assert.Equal(createDto.SubjectName, returnedSubject.SubjectName);
        Assert.Equal(now, returnedSubject.CreatedDate);
        Assert.Equal(userName, returnedSubject.CreatedBy);
        Assert.Null(returnedSubject.ModifiedDate);
        Assert.Null(returnedSubject.ModifiedBy);
    }

    [Fact]
    public async Task UpdateSubject_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var userName = "testuser";
        var updateDto = new UpdateSubjectDto { SubjectName = "Updated C#" };
        var existingSubject = new Subject {
            ID = 1,
            SubjectName = "C#",
            CreatedDate = now.AddDays(-1),
            CreatedBy = userName,
            ModifiedDate = null,
            ModifiedBy = null,
            Topics = new List<Topic>()
        };
        var updatedSubject = new Subject {
            ID = 1,
            SubjectName = "Updated C#",
            CreatedDate = now.AddDays(-1),
            CreatedBy = userName,
            ModifiedDate = now,
            ModifiedBy = userName,
            Topics = new List<Topic>()
        };
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(existingSubject);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<LogContext>(), It.IsAny<Subject>())).ReturnsAsync(updatedSubject);

        // Act
        var result = await _controller.UpdateSubject(1, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<LogContext>(), It.IsAny<Subject>()), Times.Once);
        Assert.Equal(now, updatedSubject.ModifiedDate);
        Assert.Equal(userName, updatedSubject.ModifiedBy);
    }

    [Fact]
    public async Task UpdateSubject_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateSubjectDto { SubjectName = "Updated C#" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 999))
            .ReturnsAsync((Subject)null);

        // Act
        var result = await _controller.UpdateSubject(999, updateDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteSubject_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var subject = new Subject { ID = 1, SubjectName = "C#" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(),1)).ReturnsAsync(subject);

        // Act
        var result = await _controller.DeleteSubject(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.DeleteAsync(It.IsAny<LogContext>(),1), Times.Once);
    }

    [Fact]
    public async Task DeleteSubject_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        int invalidId = 999;
        _mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<LogContext>(), invalidId))
            .ThrowsAsync(new SubjectNotFoundException(invalidId));

        // Act
        var result = await _controller.DeleteSubject(invalidId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetSubjects_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<LogContext>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetSubjects();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving subjects", statusCodeResult.Value);
    }

    [Fact]
    public async Task CreateSubject_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        var createDto = new CreateSubjectDto { SubjectName = "Test Subject" };
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<LogContext>(), It.IsAny<Subject>()))
            .ThrowsAsync(new Exception("Test exception"));
        _mapper.Setup(m => m.Map<Subject>(It.IsAny<CreateSubjectDto>()))
            .Returns(new Subject { SubjectName = createDto.SubjectName });

        // Act
        var result = await _controller.CreateSubject(createDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while creating the subject", statusCodeResult.Value);
    }
    
    [Fact]
    public async Task UpdateSubject_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        var updateDto = new UpdateSubjectDto { SubjectName = "Test Subject" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1))
            .ReturnsAsync(new Subject { ID = 1, SubjectName = "Old Name" });
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<LogContext>(), It.IsAny<Subject>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.UpdateSubject(1, updateDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while updating the subject", statusCodeResult.Value);
    }

    [Fact]
    public async Task DeleteSubject_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<LogContext>(), 1))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.DeleteSubject(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while deleting the subject", statusCodeResult.Value);
    }

    [Fact]
    public async Task GetSubject_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetSubject(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving the subject", statusCodeResult.Value);
    }
}