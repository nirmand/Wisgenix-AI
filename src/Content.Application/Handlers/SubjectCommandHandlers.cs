using AutoMapper;
using Wisgenix.SharedKernel.Application;
using Wisgenix.SharedKernel.Domain.Exceptions;
using Wisgenix.SharedKernel.Infrastructure.Logging;
using Content.Application.Commands;
using Content.Application.DTOs;
using Content.Application.Queries;
using Content.Domain.Entities;
using Content.Domain.Repositories;

namespace Content.Application.Handlers;

/// <summary>
/// Handler for CreateSubjectCommand
/// </summary>
public class CreateSubjectCommandHandler : ICommandHandler<CreateSubjectCommand, GetSubjectResponse>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public CreateSubjectCommandHandler(
        ISubjectRepository subjectRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingService logger)
    {
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetSubjectResponse> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("CreateSubject");
        
        try
        {
            _logger.LogOperationStart<Subject>(logContext, "Creating subject with name: {SubjectName}", request.SubjectName);

            // Check for duplicate
            if (await _subjectRepository.ExistsByNameAsync(request.SubjectName, cancellationToken))
            {
                throw new DuplicateEntityException(nameof(Subject), nameof(Subject.SubjectName), request.SubjectName);
            }

            // Create domain entity
            var subject = new Subject(request.SubjectName);
            
            // Save to repository
            var createdSubject = await _subjectRepository.AddAsync(subject, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogOperationSuccess<Subject>(logContext, "Successfully created subject with ID: {SubjectId}", createdSubject.Id);

            // Map to response DTO
            return _mapper.Map<GetSubjectResponse>(createdSubject);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Subject>(logContext, ex, "Error creating subject");
            throw;
        }
    }
}

/// <summary>
/// Handler for UpdateSubjectCommand
/// </summary>
public class UpdateSubjectCommandHandler : ICommandHandler<UpdateSubjectCommand, GetSubjectResponse>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public UpdateSubjectCommandHandler(
        ISubjectRepository subjectRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingService logger)
    {
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetSubjectResponse> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("UpdateSubject");
        
        try
        {
            _logger.LogOperationStart<Subject>(logContext, "Updating subject with ID: {SubjectId}", request.Id);

            // Get existing subject
            var subject = await _subjectRepository.GetByIdAsync(request.Id, cancellationToken);
            if (subject == null)
            {
                throw new EntityNotFoundException(nameof(Subject), request.Id);
            }

            // Check for duplicate name (excluding current subject)
            if (await _subjectRepository.ExistsByNameAsync(request.SubjectName, request.Id, cancellationToken))
            {
                throw new DuplicateEntityException(nameof(Subject), nameof(Subject.SubjectName), request.SubjectName);
            }

            // Update domain entity
            subject.UpdateSubjectName(request.SubjectName);
            
            // Save changes
            var updatedSubject = await _subjectRepository.UpdateAsync(subject, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogOperationSuccess<Subject>(logContext, "Successfully updated subject with ID: {SubjectId}", updatedSubject.Id);

            // Map to response DTO
            return _mapper.Map<GetSubjectResponse>(updatedSubject);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Subject>(logContext, ex, "Error updating subject");
            throw;
        }
    }
}

/// <summary>
/// Handler for DeleteSubjectCommand
/// </summary>
public class DeleteSubjectCommandHandler : ICommandHandler<DeleteSubjectCommand>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggingService _logger;

    public DeleteSubjectCommandHandler(
        ISubjectRepository subjectRepository,
        IUnitOfWork unitOfWork,
        ILoggingService logger)
    {
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("DeleteSubject");

        try
        {
            _logger.LogOperationStart<Subject>(logContext, "Deleting subject with ID: {SubjectId}", request.Id);

            // Check if subject exists
            if (!await _subjectRepository.ExistsAsync(request.Id, cancellationToken))
            {
                throw new EntityNotFoundException(nameof(Subject), request.Id);
            }

            // Delete subject
            await _subjectRepository.DeleteAsync(request.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogOperationSuccess<Subject>(logContext, "Successfully deleted subject with ID: {SubjectId}", request.Id);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Subject>(logContext, ex, "Error deleting subject");
            throw;
        }
    }
}

/// <summary>
/// Handler for GetAllSubjectsQuery
/// </summary>
public class GetAllSubjectsQueryHandler : IQueryHandler<GetAllSubjectsQuery, GetSubjectsResponse>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public GetAllSubjectsQueryHandler(
        ISubjectRepository subjectRepository,
        IMapper mapper,
        ILoggingService logger)
    {
        _subjectRepository = subjectRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetSubjectsResponse> Handle(GetAllSubjectsQuery request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("GetAllSubjects");

        try
        {
            _logger.LogOperationStart<Subject>(logContext, "Retrieving all subjects");

            var subjects = request.IncludeTopics
                ? await _subjectRepository.GetSubjectsWithTopicsAsync(cancellationToken)
                : await _subjectRepository.GetAllAsync(cancellationToken);

            var subjectDtos = _mapper.Map<IEnumerable<GetSubjectResponse>>(subjects);

            _logger.LogOperationSuccess<Subject>(logContext, "Successfully retrieved {Count} subjects", subjects.Count());

            return new GetSubjectsResponse
            {
                Subjects = subjectDtos,
                TotalCount = subjects.Count()
            };
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Subject>(logContext, ex, "Error retrieving subjects");
            throw;
        }
    }
}

/// <summary>
/// Handler for GetSubjectByIdQuery
/// </summary>
public class GetSubjectByIdQueryHandler : IQueryHandler<GetSubjectByIdQuery, GetSubjectResponse?>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public GetSubjectByIdQueryHandler(
        ISubjectRepository subjectRepository,
        IMapper mapper,
        ILoggingService logger)
    {
        _subjectRepository = subjectRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetSubjectResponse?> Handle(GetSubjectByIdQuery request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("GetSubjectById");

        try
        {
            _logger.LogOperationStart<Subject>(logContext, "Retrieving subject with ID: {SubjectId}", request.Id);

            var subject = await _subjectRepository.GetByIdAsync(request.Id, cancellationToken);

            if (subject == null)
            {
                _logger.LogOperationWarning<Subject>(logContext, "Subject with ID {SubjectId} not found", request.Id);
                return null;
            }

            _logger.LogOperationSuccess<Subject>(logContext, "Successfully retrieved subject with ID: {SubjectId}", request.Id);

            return _mapper.Map<GetSubjectResponse>(subject);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Subject>(logContext, ex, "Error retrieving subject");
            throw;
        }
    }
}
