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
/// Handler for CreateQuestionCommand
/// </summary>
public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand, GetQuestionResponse>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public CreateQuestionCommandHandler(
        IQuestionRepository questionRepository,
        ITopicRepository topicRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingService logger)
    {
        _questionRepository = questionRepository;
        _topicRepository = topicRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetQuestionResponse> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("CreateQuestion");
        
        try
        {
            _logger.LogOperationStart<Question>(logContext, "Creating question for topic: {TopicId}", request.TopicId);

            // Check if topic exists
            if (!await _topicRepository.ExistsAsync(request.TopicId, cancellationToken))
            {
                throw new EntityNotFoundException(nameof(Topic), request.TopicId);
            }

            // Check for duplicate question text within the same topic
            if (await _questionRepository.ExistsByTextAndTopicAsync(request.QuestionText, request.TopicId, cancellationToken))
            {
                throw new DuplicateEntityException(nameof(Question), nameof(Question.QuestionText), request.QuestionText);
            }

            // Create domain entity
            var question = new Question(request.QuestionText, request.TopicId, request.DifficultyLevel, 
                request.MaxScore, request.GeneratedBy, request.QuestionSourceReference);
            
            // Save to repository
            var createdQuestion = await _questionRepository.AddAsync(question, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogOperationSuccess<Question>(logContext, "Successfully created question with ID: {QuestionId}", createdQuestion.Id);

            // Map to response DTO
            return _mapper.Map<GetQuestionResponse>(createdQuestion);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Question>(logContext, ex, "Error creating question");
            throw;
        }
    }
}

/// <summary>
/// Handler for UpdateQuestionCommand
/// </summary>
public class UpdateQuestionCommandHandler : ICommandHandler<UpdateQuestionCommand, GetQuestionResponse>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public UpdateQuestionCommandHandler(
        IQuestionRepository questionRepository,
        ITopicRepository topicRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingService logger)
    {
        _questionRepository = questionRepository;
        _topicRepository = topicRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetQuestionResponse> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("UpdateQuestion");
        
        try
        {
            _logger.LogOperationStart<Question>(logContext, "Updating question with ID: {QuestionId}", request.Id);

            // Get existing question
            var question = await _questionRepository.GetByIdAsync(request.Id, cancellationToken);
            if (question == null)
            {
                throw new EntityNotFoundException(nameof(Question), request.Id);
            }

            // Check if topic exists
            if (!await _topicRepository.ExistsAsync(request.TopicId, cancellationToken))
            {
                throw new EntityNotFoundException(nameof(Topic), request.TopicId);
            }

            // Check for duplicate question text within the same topic (excluding current question)
            if (await _questionRepository.ExistsByTextAndTopicAsync(request.QuestionText, request.TopicId, request.Id, cancellationToken))
            {
                throw new DuplicateEntityException(nameof(Question), nameof(Question.QuestionText), request.QuestionText);
            }

            // Update domain entity
            question.UpdateQuestion(request.QuestionText, request.DifficultyLevel, request.MaxScore, 
                request.GeneratedBy, request.QuestionSourceReference);
            
            // Save changes
            var updatedQuestion = await _questionRepository.UpdateAsync(question, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogOperationSuccess<Question>(logContext, "Successfully updated question with ID: {QuestionId}", updatedQuestion.Id);

            // Map to response DTO
            return _mapper.Map<GetQuestionResponse>(updatedQuestion);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Question>(logContext, ex, "Error updating question");
            throw;
        }
    }
}

/// <summary>
/// Handler for DeleteQuestionCommand
/// </summary>
public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggingService _logger;

    public DeleteQuestionCommandHandler(
        IQuestionRepository questionRepository,
        IUnitOfWork unitOfWork,
        ILoggingService logger)
    {
        _questionRepository = questionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("DeleteQuestion");
        
        try
        {
            _logger.LogOperationStart<Question>(logContext, "Deleting question with ID: {QuestionId}", request.Id);

            // Check if question exists
            if (!await _questionRepository.ExistsAsync(request.Id, cancellationToken))
            {
                throw new EntityNotFoundException(nameof(Question), request.Id);
            }

            // Delete question
            await _questionRepository.DeleteAsync(request.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogOperationSuccess<Question>(logContext, "Successfully deleted question with ID: {QuestionId}", request.Id);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Question>(logContext, ex, "Error deleting question");
            throw;
        }
    }
}

/// <summary>
/// Handler for GetAllQuestionsQuery
/// </summary>
public class GetAllQuestionsQueryHandler : IQueryHandler<GetAllQuestionsQuery, GetQuestionsResponse>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public GetAllQuestionsQueryHandler(
        IQuestionRepository questionRepository,
        IMapper mapper,
        ILoggingService logger)
    {
        _questionRepository = questionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetQuestionsResponse> Handle(GetAllQuestionsQuery request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("GetAllQuestions");

        try
        {
            _logger.LogOperationStart<Question>(logContext, "Retrieving all questions");

            var questions = request.IncludeOptions
                ? await _questionRepository.GetQuestionsWithOptionsAsync(cancellationToken)
                : await _questionRepository.GetAllAsync(cancellationToken);

            var questionDtos = _mapper.Map<IEnumerable<GetQuestionResponse>>(questions);

            _logger.LogOperationSuccess<Question>(logContext, "Successfully retrieved {Count} questions", questions.Count());

            return new GetQuestionsResponse
            {
                Questions = questionDtos,
                TotalCount = questions.Count()
            };
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Question>(logContext, ex, "Error retrieving questions");
            throw;
        }
    }
}

/// <summary>
/// Handler for GetQuestionByIdQuery
/// </summary>
public class GetQuestionByIdQueryHandler : IQueryHandler<GetQuestionByIdQuery, GetQuestionResponse?>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public GetQuestionByIdQueryHandler(
        IQuestionRepository questionRepository,
        IMapper mapper,
        ILoggingService logger)
    {
        _questionRepository = questionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetQuestionResponse?> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("GetQuestionById");

        try
        {
            _logger.LogOperationStart<Question>(logContext, "Retrieving question with ID: {QuestionId}", request.Id);

            var question = await _questionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (question == null)
            {
                _logger.LogOperationWarning<Question>(logContext, "Question with ID {QuestionId} not found", request.Id);
                return null;
            }

            _logger.LogOperationSuccess<Question>(logContext, "Successfully retrieved question with ID: {QuestionId}", request.Id);

            return _mapper.Map<GetQuestionResponse>(question);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Question>(logContext, ex, "Error retrieving question");
            throw;
        }
    }
}

/// <summary>
/// Handler for GetQuestionsByTopicIdQuery
/// </summary>
public class GetQuestionsByTopicIdQueryHandler : IQueryHandler<GetQuestionsByTopicIdQuery, GetQuestionsResponse>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public GetQuestionsByTopicIdQueryHandler(
        IQuestionRepository questionRepository,
        IMapper mapper,
        ILoggingService logger)
    {
        _questionRepository = questionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetQuestionsResponse> Handle(GetQuestionsByTopicIdQuery request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("GetQuestionsByTopicId");

        try
        {
            _logger.LogOperationStart<Question>(logContext, "Retrieving questions for topic ID: {TopicId}", request.TopicId);

            var questions = await _questionRepository.GetByTopicIdAsync(request.TopicId, cancellationToken);
            var questionDtos = _mapper.Map<IEnumerable<GetQuestionResponse>>(questions);

            _logger.LogOperationSuccess<Question>(logContext, "Successfully retrieved {Count} questions for topic ID: {TopicId}",
                questions.Count(), request.TopicId);

            return new GetQuestionsResponse
            {
                Questions = questionDtos,
                TotalCount = questions.Count()
            };
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<Question>(logContext, ex, "Error retrieving questions by topic ID");
            throw;
        }
    }
}
