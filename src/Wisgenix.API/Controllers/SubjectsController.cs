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
    public class SubjectsController : BaseApiController
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
            var logContext = CreateLogContext("GetSubjects");
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
            var logContext = CreateLogContext("GetSubject");
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(logContext, id);
                var subjectDto = _mapper.Map<SubjectDto>(subject);
                return Ok(subjectDto);
            }
            catch (SubjectNotFoundException ex)
            {
                _logger.LogOperationWarning<Subject>(logContext, ex.Message);
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
            var logContext = CreateLogContext("CreateSubject", userName);
            try
            {
                var subject = _mapper.Map<Subject>(createSubjectDto);
                var createdSubject = await _subjectRepository.CreateAsync(logContext, subject);
                var subjectDto = _mapper.Map<SubjectDto>(createdSubject);
                return CreatedAtAction(nameof(GetSubject), new { id = createdSubject.ID }, subjectDto);
            }
            catch (DuplicateSubjectException ex)
            {
                _logger.LogOperationWarning<Subject>(logContext, ex.Message);
                return BadRequest(new { message = ex.Message });
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
            var logContext = CreateLogContext("UpdateSubject", userName);
            try
            {
                var existingSubject = await _subjectRepository.GetByIdAsync(logContext, id);
                if (existingSubject == null)
                {
                    _logger.LogOperationWarning<Subject>(logContext, $"Subject with ID {id} was not found");
                    return NotFound($"Subject with ID {id} was not found");
                }
                _mapper.Map(updateSubjectDto, existingSubject);
                await _subjectRepository.UpdateAsync(logContext, existingSubject);
                return NoContent();
            }
            catch (DuplicateSubjectException ex)
            {
                _logger.LogOperationWarning<Subject>(logContext, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (SubjectNotFoundException ex)
            {
                _logger.LogOperationWarning<Subject>(logContext, ex.Message);
                return BadRequest(ex.Message);
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
            var logContext = CreateLogContext("DeleteSubject");
            try
            {
                _logger.LogInformation($"Deleting subject with ID: {id}");
                await _subjectRepository.DeleteAsync(logContext, id);
                _logger.LogInformation($"Successfully deleted subject with ID: {id}");
                return NoContent();
            }
            catch (SubjectNotFoundException ex)
            {
                _logger.LogOperationWarning<Subject>(logContext, ex.Message);
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