using AIUpskillingPlatform.DTO;
using AIUpskillingPlatform.Core.Logger;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIUpskillingPlatform.API.Controllers
{
    [ApiController]
    [Route("api/content")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly ILoggingService _logger;

        public SubjectsController(ISubjectRepository subjectRepository, ILoggingService logger)
        {
            _subjectRepository = subjectRepository;
            _logger = logger;
        }

        [HttpGet("get-subjects")]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjects()
        {
            LogContext logContext = LogContext.Create("GetSubjects");
            try
            {
                _logger.LogInformation("Getting all subjects");
                var subjects = await _subjectRepository.GetAllAsync(logContext);
                var subjectDtos = subjects.Select(s => new SubjectDto
                {
                    ID = s.ID,
                    SubjectName = s.SubjectName,
                    Topics = s.Topics.Select(t => new TopicDto
                    {
                        ID = t.ID,
                        TopicName = t.TopicName
                    }).ToList()
                });
                _logger.LogInformation("Successfully retrieved all subjects");
                return Ok(subjectDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all subjects");
                return StatusCode(500, "An error occurred while retrieving subjects");
            }
        }

        [HttpGet("get-subject/{id}")]
        public async Task<ActionResult<SubjectDto>> GetSubject(int id)
        {
            LogContext logContext = LogContext.Create("GetSubject");
            try
            {
                _logger.LogInformation($"Getting subject {id}");
                var subject = await _subjectRepository.GetByIdAsync(logContext,id);
                if (subject == null)
                {
                    return NotFound($"Subject with ID {id} was not found");
                }

                var subjectDto = new SubjectDto
                {
                    ID = subject.ID,
                    SubjectName = subject.SubjectName,
                    Topics = subject.Topics.Select(t => new TopicDto
                    {
                        ID = t.ID,
                        TopicName = t.TopicName
                    }).ToList()
                };
                _logger.LogInformation($"Found subject with {id}");
                return Ok(subjectDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting subject with ID: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the subject");
            }
        }

        [HttpPost("create-subject")]
        public async Task<ActionResult<SubjectDto>> CreateSubject(CreateSubjectDto createSubjectDto)
        {
            LogContext logContext = LogContext.Create("CreateSubject");
            try
            {
                var subject = new Subject
                {
                    SubjectName = createSubjectDto.SubjectName
                };

                var createdSubject = await _subjectRepository.CreateAsync(logContext,subject);
                var subjectDto = new SubjectDto
                {
                    ID = createdSubject.ID,
                    SubjectName = createdSubject.SubjectName
                };

                return CreatedAtAction(nameof(GetSubject), new { id = createdSubject.ID }, subjectDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating subject");
                return StatusCode(500, "An error occurred while creating the subject");
            }
        }

        [HttpPut("update-subject/{id}")]
        public async Task<IActionResult> UpdateSubject(int id, UpdateSubjectDto updateSubjectDto)
        {
            LogContext logContext = LogContext.Create("UpdateSubject");
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(logContext,id);
                if (subject == null)
                {
                    return NotFound($"Subject with ID {id} was not found.");
                }

                subject.SubjectName = updateSubjectDto.SubjectName;
                await _subjectRepository.UpdateAsync(logContext,subject);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating subject with ID: {Id}", id);
                return StatusCode(500, "An error occurred while updating the subject");
            }
        }

        [HttpDelete("delete-subject/{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            LogContext logContext = LogContext.Create("DeleteSubject");
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(logContext,id);
                if (subject == null)
                {
                    return NotFound($"Subject with ID {id} was not found.");
                }

                await _subjectRepository.DeleteAsync(logContext,id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting subject with ID: {Id}", id);
                return StatusCode(500, "An error occurred while deleting the subject");
            }
        }
    }
}