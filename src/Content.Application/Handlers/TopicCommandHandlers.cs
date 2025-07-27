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
/// Handler for CreateTopicCommand
/// </summary>
public class CreateTopicCommandHandler : ICommandHandler<CreateTopicCommand, GetTopicResponse>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public CreateTopicCommandHandler(
        ITopicRepository topicRepository,
        ISubjectRepository subjectRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingService logger)
    {
        _topicRepository = topicRepository;
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetTopicResponse> Handle(CreateTopicCommand request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("CreateTopic");
        
        try
        {
            _logger.LogOperationStart<Topic>(logContext, "Creating topic with name: {TopicName} for subject: {SubjectId}", 
                request.TopicName, request.SubjectId);

            // Check if subject exists
            if (!await _subjectRepository.ExistsAsync(request.SubjectId, cancellationToken))
            {
                throw new EntityNotFoundException(nameof(Subject), request.SubjectId);
            }

            // Check for duplicate topic name within the same subject
            if (await _topicRepository.ExistsByNameAndSubjectAsync(request.TopicName, request.SubjectId, cancellationToken))
            {
                throw new DuplicateEntityException(nameof(Topic), nameof(Topic.TopicName), request.TopicName);
            }

            // Create domain entity
            var topic = new Topic(request.TopicName, request.SubjectId);
            
            // Save to repository
            var createdTopic = await _topicRepository.AddAsync(topic, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogOperationSuccess<Topic>(logContext, "Successfully created topic with ID: {TopicId}", createdTopic.Id);

            // Map to response DTO
            return _mapper.Map<GetTopicResponse>(createdTopic);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, "Error creating topic");
            throw;
        }
    }
}

/// <summary>
/// Handler for UpdateTopicCommand
/// </summary>
public class UpdateTopicCommandHandler : ICommandHandler<UpdateTopicCommand, GetTopicResponse>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public UpdateTopicCommandHandler(
        ITopicRepository topicRepository,
        ISubjectRepository subjectRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingService logger)
    {
        _topicRepository = topicRepository;
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetTopicResponse> Handle(UpdateTopicCommand request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("UpdateTopic");
        
        try
        {
            _logger.LogOperationStart<Topic>(logContext, "Updating topic with ID: {TopicId}", request.Id);

            // Get existing topic
            var topic = await _topicRepository.GetByIdAsync(request.Id, cancellationToken);
            if (topic == null)
            {
                throw new EntityNotFoundException(nameof(Topic), request.Id);
            }

            // Check if subject exists
            if (!await _subjectRepository.ExistsAsync(request.SubjectId, cancellationToken))
            {
                throw new EntityNotFoundException(nameof(Subject), request.SubjectId);
            }

            // Check for duplicate name within the same subject (excluding current topic)
            if (await _topicRepository.ExistsByNameAndSubjectAsync(request.TopicName, request.SubjectId, request.Id, cancellationToken))
            {
                throw new DuplicateEntityException(nameof(Topic), nameof(Topic.TopicName), request.TopicName);
            }

            // Update domain entity
            topic.UpdateTopicName(request.TopicName);
            
            // Save changes
            var updatedTopic = await _topicRepository.UpdateAsync(topic, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogOperationSuccess<Topic>(logContext, "Successfully updated topic with ID: {TopicId}", updatedTopic.Id);

            // Map to response DTO
            return _mapper.Map<GetTopicResponse>(updatedTopic);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, "Error updating topic");
            throw;
        }
    }
}

/// <summary>
/// Handler for DeleteTopicCommand
/// </summary>
public class DeleteTopicCommandHandler : ICommandHandler<DeleteTopicCommand>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggingService _logger;

    public DeleteTopicCommandHandler(
        ITopicRepository topicRepository,
        IUnitOfWork unitOfWork,
        ILoggingService logger)
    {
        _topicRepository = topicRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeleteTopicCommand request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("DeleteTopic");
        
        try
        {
            _logger.LogOperationStart<Topic>(logContext, "Deleting topic with ID: {TopicId}", request.Id);

            // Check if topic exists
            if (!await _topicRepository.ExistsAsync(request.Id, cancellationToken))
            {
                throw new EntityNotFoundException(nameof(Topic), request.Id);
            }

            // Delete topic
            await _topicRepository.DeleteAsync(request.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogOperationSuccess<Topic>(logContext, "Successfully deleted topic with ID: {TopicId}", request.Id);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, "Error deleting topic");
            throw;
        }
    }
}

/// <summary>
/// Handler for GetAllTopicsQuery
/// </summary>
public class GetAllTopicsQueryHandler : IQueryHandler<GetAllTopicsQuery, GetTopicsResponse>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public GetAllTopicsQueryHandler(
        ITopicRepository topicRepository,
        IMapper mapper,
        ILoggingService logger)
    {
        _topicRepository = topicRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetTopicsResponse> Handle(GetAllTopicsQuery request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("GetAllTopics");

        try
        {
            _logger.LogOperationStart<Topic>(logContext, "Retrieving all topics");

            var topics = request.IncludeQuestions
                ? await _topicRepository.GetTopicsWithQuestionsAsync(cancellationToken)
                : await _topicRepository.GetAllAsync(cancellationToken);

            var topicDtos = _mapper.Map<IEnumerable<GetTopicResponse>>(topics);

            _logger.LogOperationSuccess<Topic>(logContext, "Successfully retrieved {Count} topics", topics.Count());

            return new GetTopicsResponse
            {
                Topics = topicDtos,
                TotalCount = topics.Count()
            };
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, "Error retrieving topics");
            throw;
        }
    }
}

/// <summary>
/// Handler for GetTopicByIdQuery
/// </summary>
public class GetTopicByIdQueryHandler : IQueryHandler<GetTopicByIdQuery, GetTopicResponse?>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public GetTopicByIdQueryHandler(
        ITopicRepository topicRepository,
        IMapper mapper,
        ILoggingService logger)
    {
        _topicRepository = topicRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetTopicResponse?> Handle(GetTopicByIdQuery request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("GetTopicById");

        try
        {
            _logger.LogOperationStart<Topic>(logContext, "Retrieving topic with ID: {TopicId}", request.Id);

            var topic = await _topicRepository.GetByIdAsync(request.Id, cancellationToken);

            if (topic == null)
            {
                _logger.LogOperationWarning<Topic>(logContext, "Topic with ID {TopicId} not found", request.Id);
                return null;
            }

            _logger.LogOperationSuccess<Topic>(logContext, "Successfully retrieved topic with ID: {TopicId}", request.Id);

            return _mapper.Map<GetTopicResponse>(topic);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, "Error retrieving topic");
            throw;
        }
    }
}

/// <summary>
/// Handler for GetTopicsBySubjectIdQuery
/// </summary>
public class GetTopicsBySubjectIdQueryHandler : IQueryHandler<GetTopicsBySubjectIdQuery, GetTopicsResponse>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public GetTopicsBySubjectIdQueryHandler(
        ITopicRepository topicRepository,
        IMapper mapper,
        ILoggingService logger)
    {
        _topicRepository = topicRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetTopicsResponse> Handle(GetTopicsBySubjectIdQuery request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("GetTopicsBySubjectId");

        try
        {
            _logger.LogOperationStart<Topic>(logContext, "Retrieving topics for subject ID: {SubjectId}", request.SubjectId);

            var topics = request.IncludeQuestions
                ? await _topicRepository.GetTopicsWithQuestionsBySubjectIdAsync(request.SubjectId, cancellationToken)
                : await _topicRepository.GetBySubjectIdAsync(request.SubjectId, cancellationToken);

            var topicDtos = _mapper.Map<IEnumerable<GetTopicResponse>>(topics);

            _logger.LogOperationSuccess<Topic>(logContext, "Successfully retrieved {Count} topics for subject ID: {SubjectId}",
                topics.Count(), request.SubjectId);

            return new GetTopicsResponse
            {
                Topics = topicDtos,
                TotalCount = topics.Count()
            };
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Topic>(logContext, ex, "Error retrieving topics for subject ID: {SubjectId}", request.SubjectId);
            throw;
        }
    }
}
