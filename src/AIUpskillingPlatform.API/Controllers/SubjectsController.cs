using AutoMapper;
using AIUpskillingPlatform.DTO;
using AIUpskillingPlatform.Core.Logger;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.Repositories.Interfaces;
using AIUpskillingPlatform.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AIUpskillingPlatform.API.Controllers
{
    [ApiController]
    [Route("api/content")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly ILoggingService _logger;
        private readonly IMapper _mapper;

        public SubjectsController(ISubjectRepository subjectRepository, ILoggingService logger, IMapper mapper)
        {
            _subjectRepository = subjectRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("subjects")]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjects()
        {
            LogContext logContext = LogContext.Create("GetSubjects");
            try
            {
                var subjects = await _subjectRepository.GetAllAsync(logContext);
                var subjectDtos = _mapper.Map<IEnumerable<SubjectDto>>(subjects);
                return Ok(subjectDtos);
            }
            catch (Exception ex)
            {
                _logger.LogOperationError<Subject>(logContext, ex, "Unhandled error getting all subjects");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while retrieving subjects");
            }
        }

        [HttpGet("subject/{id}")]
        public async Task<ActionResult<SubjectDto>> GetSubject(int id)
        {
            LogContext logContext = LogContext.Create("GetSubject");
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(logContext, id);
                var subjectDto = _mapper.Map<SubjectDto>(subject);
                return Ok(subjectDto);
            }
            catch (SubjectNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogOperationError<Subject>(logContext, ex, $"Unhandled error getting subject {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while retrieving the subject");
            }
        }

        [HttpPost("create-subject")]
        public async Task<ActionResult<SubjectDto>> CreateSubject([FromBody] CreateSubjectDto createSubjectDto)
        {
            LogContext logContext = LogContext.Create("CreateSubject");
            try
            {
                var subject = _mapper.Map<Subject>(createSubjectDto);
                var createdSubject = await _subjectRepository.CreateAsync(logContext, subject);
                var subjectDto = _mapper.Map<SubjectDto>(createdSubject);

                return CreatedAtAction(nameof(GetSubject), new { id = createdSubject.ID }, subjectDto);
            }
            catch (Exception ex)
            {
                _logger.LogOperationError<Subject>(logContext, ex, "Error creating subject");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while creating the subject");
            }
        }

        [HttpPut("update-subject/{id}")]
        public async Task<IActionResult> UpdateSubject(int id, UpdateSubjectDto updateSubjectDto)
        {
            LogContext logContext = LogContext.Create("UpdateSubject");
            try
            {
                var existingSubject = await _subjectRepository.GetByIdAsync(logContext, id);
                if (existingSubject == null)
                {
                    return NotFound($"Subject with ID {id} was not found.");
                }

                _mapper.Map(updateSubjectDto, existingSubject);
                await _subjectRepository.UpdateAsync(logContext, existingSubject);
                return NoContent();
            }
            catch (SubjectNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogOperationError<Subject>(logContext, ex, $"Error updating subject {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while updating the subject");
            }
        }

        [HttpDelete("delete-subject/{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            LogContext logContext = LogContext.Create("DeleteSubject");
            try
            {
                await _subjectRepository.DeleteAsync(logContext, id);
                return NoContent();
            }
            catch (SubjectNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogOperationError<Subject>(logContext, ex, $"Error deleting subject {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while deleting the subject");
            }
        }
    }
}