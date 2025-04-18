using AIUpskillingPlatform.API.Controllers;
using AIUpskillingPlatform.API.DTOs;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using AIUpskillingPlatform.Core.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AIUpskillingPlatform.Tests.Controllers;

public class SubjectsControllerTests
{
    private readonly Mock<ISubjectRepository> _mockRepository;
    private readonly Mock<ILoggingService> _mockLogger;
    private readonly SubjectsController _controller;

    public SubjectsControllerTests()
    {
        _mockRepository = new Mock<ISubjectRepository>();
        _mockLogger = new Mock<ILoggingService>();
        _controller = new SubjectsController(_mockRepository.Object, _mockLogger.Object);
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
        _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(subjects);

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
        _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(subject);

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
        _mockRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Subject?)null);

        // Act
        var result = await _controller.GetSubject(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateSubject_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateSubjectDto { SubjectName = "C#" };
        var createdSubject = new Subject { ID = 1, SubjectName = "C#" };
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Subject>())).ReturnsAsync(createdSubject);

        // Act
        var result = await _controller.CreateSubject(createDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedSubject = Assert.IsType<SubjectDto>(createdAtActionResult.Value);
        Assert.Equal(createDto.SubjectName, returnedSubject.SubjectName);
    }

    [Fact]
    public async Task UpdateSubject_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var updateDto = new UpdateSubjectDto { SubjectName = "Updated C#" };
        var existingSubject = new Subject { ID = 1, SubjectName = "C#" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingSubject);

        // Act
        var result = await _controller.UpdateSubject(1, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Subject>()), Times.Once);
    }

    [Fact]
    public async Task DeleteSubject_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var subject = new Subject { ID = 1, SubjectName = "C#" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(subject);

        // Act
        var result = await _controller.DeleteSubject(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }
} 