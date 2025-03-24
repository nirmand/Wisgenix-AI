using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIUpskillingPlatform.API.DTOs;
using AIUpskillingPlatform.Repositories.Interfaces;
using AIUpskillingPlatform.Data.Entities;
namespace AIUpskillingPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly ILogger<SubjectsController> _logger;

        public SubjectsController(ISubjectRepository subjectRepository, ILogger<SubjectsController> logger)
        {
            _subjectRepository = subjectRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjects()
        {
            try
            {
                var subjects = await _subjectRepository.GetAllAsync();
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
                return Ok(subjectDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all subjects");
                return StatusCode(500, "An error occurred while retrieving subjects");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDto>> GetSubject(int id)
        {
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(id);
                if (subject == null)
                {
                    return NotFound($"Subject with ID {id} was not found.");
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
                return Ok(subjectDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting subject with ID: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the subject");
            }
        }

        [HttpPost]
        public async Task<ActionResult<SubjectDto>> CreateSubject(CreateSubjectDto createSubjectDto)
        {
            try
            {
                var subject = new Subject
                {
                    SubjectName = createSubjectDto.SubjectName
                };

                var createdSubject = await _subjectRepository.CreateAsync(subject);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, UpdateSubjectDto updateSubjectDto)
        {
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(id);
                if (subject == null)
                {
                    return NotFound($"Subject with ID {id} was not found.");
                }

                subject.SubjectName = updateSubjectDto.SubjectName;
                await _subjectRepository.UpdateAsync(subject);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating subject with ID: {Id}", id);
                return StatusCode(500, "An error occurred while updating the subject");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(id);
                if (subject == null)
                {
                    return NotFound($"Subject with ID {id} was not found.");
                }

                await _subjectRepository.DeleteAsync(id);
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