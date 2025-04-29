using AutoMapper;
using AIUpskillingPlatform.API.Controllers;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using AIUpskillingPlatform.Core.Logger;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using AIUpskillingPlatform.DTO;
using FluentValidation;
using AIUpskillingPlatform.DTO.Validators;

namespace AIUpskillingPlatform.Tests.Controllers;

public class SubjectsControllerTests
{
    private readonly Mock<ISubjectRepository> _mockRepository;
    private readonly Mock<ILoggingService> _mockLogger;
    private readonly SubjectsController _controller;
    private readonly Mock<IMapper> _mapper;
    private readonly CreateSubjectDtoValidator _createSubjectValidator = new CreateSubjectDtoValidator();
    private readonly UpdateSubjectDtoValidator _updateSubjectValidator = new UpdateSubjectDtoValidator();


    public SubjectsControllerTests()
    {
        _mockRepository = new Mock<ISubjectRepository>();
        _mockLogger = new Mock<ILoggingService>();
        _mapper = new Mock<IMapper>();
        _controller = new SubjectsController(_mockRepository.Object, _mockLogger.Object, _mapper.Object);
        
        // Add these mapper configurations
        _mapper.Setup(m => m.Map<SubjectDto>(It.IsAny<Subject>()))
        .Returns((Subject source) => new SubjectDto 
        { 
            ID = source.ID, 
            SubjectName = source.SubjectName 
        });
        
        _mapper.Setup(m => m.Map<IEnumerable<SubjectDto>>(It.IsAny<IEnumerable<Subject>>()))
        .Returns((IEnumerable<Subject> source) => source.Select(s => new SubjectDto 
        { 
            ID = s.ID, 
            SubjectName = s.SubjectName 
        }));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CreateSubject_WithInvalidName_ReturnsBadRequest(string subjectName)
    {
        // Arrange
        var createDto = new CreateSubjectDto { SubjectName = subjectName };

        // Validate using the validator directly
        var validationResult = await _createSubjectValidator.ValidateAsync(createDto);

        // If validation fails, the controller should return BadRequest
        if (!validationResult.IsValid)
        {
            // Act
            var result = await _controller.CreateSubject(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Subject name is required", badRequestResult.Value.ToString());
        }
        else
        {
            Assert.True(false, "Validation should have failed for missing subject name");
        }        
    }

    [Fact]
    public async Task CreateSubject_WithTooLongName_ReturnsBadRequest()
    {
        // Arrange
        string longName = new string('a', 101); // Create a string longer than 100 characters
        var createDto = new CreateSubjectDto { SubjectName = longName };
        _mapper.Setup(m => m.Map<Subject>(It.IsAny<CreateSubjectDto>()))
            .Returns(new Subject { SubjectName = longName });

        // Act
        var result = await _controller.CreateSubject(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Subject name must not exceed 100 characters", badRequestResult.Value);
    }

    [Theory]
    [InlineData("Subject@Name")]
    [InlineData("Subject#123")]
    [InlineData("Subject-Name")]
    public async Task CreateSubject_WithInvalidCharacters_ReturnsBadRequest(string subjectName)
    {
        // Arrange
        var createDto = new CreateSubjectDto { SubjectName = subjectName };
        _mapper.Setup(m => m.Map<Subject>(It.IsAny<CreateSubjectDto>()))
            .Returns(new Subject { SubjectName = subjectName });

        // Act
        var result = await _controller.CreateSubject(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Subject name contains invalid characters", badRequestResult.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task UpdateSubject_WithInvalidName_ReturnsBadRequest(string subjectName)
    {
        // Arrange
        var updateDto = new UpdateSubjectDto { SubjectName = subjectName };
        var existingSubject = new Subject { ID = 1, SubjectName = "Original Name" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(existingSubject);
        _mapper.Setup(m => m.Map(It.IsAny<UpdateSubjectDto>(), It.IsAny<Subject>()))
            .Callback<UpdateSubjectDto, Subject>((dto, subject) => subject.SubjectName = dto.SubjectName);

        // Act
        var result = await _controller.UpdateSubject(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Subject name is required", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateSubject_WithTooLongName_ReturnsBadRequest()
    {
        // Arrange
        string longName = new string('a', 101);
        var updateDto = new UpdateSubjectDto { SubjectName = longName };
        var existingSubject = new Subject { ID = 1, SubjectName = "Original Name" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(existingSubject);
        _mapper.Setup(m => m.Map(It.IsAny<UpdateSubjectDto>(), It.IsAny<Subject>()))
            .Callback<UpdateSubjectDto, Subject>((dto, subject) => subject.SubjectName = dto.SubjectName);

        // Act
        var result = await _controller.UpdateSubject(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Subject name must not exceed 100 characters", badRequestResult.Value);
    }

    [Theory]
    [InlineData("Subject@Name")]
    [InlineData("Subject#123")]
    [InlineData("Subject-Name")]
    public async Task UpdateSubject_WithInvalidCharacters_ReturnsBadRequest(string subjectName)
    {
        // Arrange
        var updateDto = new UpdateSubjectDto { SubjectName = subjectName };
        var existingSubject = new Subject { ID = 1, SubjectName = "Original Name" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(), 1)).ReturnsAsync(existingSubject);
        _mapper.Setup(m => m.Map(It.IsAny<UpdateSubjectDto>(), It.IsAny<Subject>()))
            .Callback<UpdateSubjectDto, Subject>((dto, subject) => subject.SubjectName = dto.SubjectName);

        // Act
        var result = await _controller.UpdateSubject(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Subject name contains invalid characters", badRequestResult.Value);
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
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(),999)).ReturnsAsync((Subject?)null);

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
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<LogContext>(),It.IsAny<Subject>())).ReturnsAsync(createdSubject);

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
        _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<LogContext>(),1)).ReturnsAsync(existingSubject);

        // Act
        var result = await _controller.UpdateSubject(1, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<LogContext>(),It.IsAny<Subject>()), Times.Once);
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
}