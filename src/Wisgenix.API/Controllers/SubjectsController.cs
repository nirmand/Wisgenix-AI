using AutoMapper;
using Wisgenix.DTO;
using Wisgenix.Core.Logger;
using Wisgenix.Data.Entities;
using Wisgenix.Repositories.Interfaces;
using Wisgenix.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Wisgenix.API.Controllers
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
            var userName = User?.Identity?.Name ?? "system";
            LogContext logContext = LogContext.Create("CreateSubject", userName);
            try
            {
                var subject = _mapper.Map<Subject>(createSubjectDto);
                var createdSubject = await _subjectRepository.CreateAsync(logContext, subject);
                var subjectDto = _mapper.Map<SubjectDto>(createdSubject);

                return CreatedAtAction(nameof(GetSubject), new { id = createdSubject.ID }, subjectDto);
            }
            catch (DuplicateSubjectException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogOperationError<Subject>(logContext, ex, "Error occurred while creating subject");
                return StatusCode(500, "An error occurred while creating the subject");
            }
        }

        [HttpPut("update-subject/{id}")]
        public async Task<IActionResult> UpdateSubject(int id, UpdateSubjectDto updateSubjectDto)
        {
            var userName = User?.Identity?.Name ?? "system";
            LogContext logContext = LogContext.Create("UpdateSubject", userName);
            try
            {
                var existingSubject = await _subjectRepository.GetByIdAsync(logContext, id);
                if (existingSubject == null)
                {
                    return NotFound($"Subject with ID {id} was not found");
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
                _logger.LogOperationError<Subject>(logContext, ex, $"Error occurred while updating subject with ID: {id}");
                return StatusCode(500, "An error occurred while updating the subject");
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