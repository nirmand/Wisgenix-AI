using AutoMapper;
using Microsoft.Extensions.Logging;
using Content.Application.Commands;
using Content.Application.Queries;
using Content.Application.DTOs;
using Content.Domain.Entities;
using Content.Domain.Repositories;
using Wisgenix.SharedKernel.Application;
using Wisgenix.SharedKernel.Domain.Exceptions;
using Wisgenix.SharedKernel.Infrastructure.Logging;

namespace Content.Application.Handlers;

/// <summary>
/// Handler for creating question options
/// </summary>
public class CreateQuestionOptionCommandHandler : ICommandHandler<CreateQuestionOptionCommand, GetQuestionOptionResponse>
{
    private readonly IQuestionOptionRepository _questionOptionRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateQuestionOptionCommandHandler> _logger;

    public CreateQuestionOptionCommandHandler(
        IQuestionOptionRepository questionOptionRepository,
        IQuestionRepository questionRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateQuestionOptionCommandHandler> logger)
    {
        _questionOptionRepository = questionOptionRepository;
        _questionRepository = questionRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetQuestionOptionResponse> Handle(CreateQuestionOptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting CreateQuestionOption: Creating question option for question {QuestionId}", request.QuestionId);

        // Verify question exists
        if (!await _questionRepository.ExistsAsync(request.QuestionId, cancellationToken))
        {
            _logger.LogWarning("Question not found: {QuestionId}", request.QuestionId);
            throw new EntityNotFoundException(nameof(Question), request.QuestionId);
        }

        // Create question option
        var questionOption = new QuestionOption(request.OptionText, request.QuestionId, request.IsCorrect);

        // Save to repository
        var createdOption = await _questionOptionRepository.AddAsync(questionOption, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Success: Successfully created question option {OptionId} for question {QuestionId}", 
            createdOption.Id, request.QuestionId);

        return _mapper.Map<GetQuestionOptionResponse>(createdOption);
    }
}

/// <summary>
/// Handler for updating question options
/// </summary>
public class UpdateQuestionOptionCommandHandler : ICommandHandler<UpdateQuestionOptionCommand, GetQuestionOptionResponse>
{
    private readonly IQuestionOptionRepository _questionOptionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateQuestionOptionCommandHandler> _logger;

    public UpdateQuestionOptionCommandHandler(
        IQuestionOptionRepository questionOptionRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateQuestionOptionCommandHandler> logger)
    {
        _questionOptionRepository = questionOptionRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetQuestionOptionResponse> Handle(UpdateQuestionOptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting UpdateQuestionOption: Updating question option {OptionId}", request.Id);

        // Get existing question option
        var existingOption = await _questionOptionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingOption == null)
        {
            _logger.LogWarning("Question option not found: {OptionId}", request.Id);
            throw new EntityNotFoundException(nameof(QuestionOption), request.Id);
        }

        // Update question option
        existingOption.UpdateOption(request.OptionText, request.IsCorrect);

        // Save changes
        var updatedOption = await _questionOptionRepository.UpdateAsync(existingOption, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Success: Successfully updated question option {OptionId}", request.Id);

        return _mapper.Map<GetQuestionOptionResponse>(updatedOption);
    }
}

/// <summary>
/// Handler for deleting question options
/// </summary>
public class DeleteQuestionOptionCommandHandler : ICommandHandler<DeleteQuestionOptionCommand>
{
    private readonly IQuestionOptionRepository _questionOptionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteQuestionOptionCommandHandler> _logger;

    public DeleteQuestionOptionCommandHandler(
        IQuestionOptionRepository questionOptionRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteQuestionOptionCommandHandler> logger)
    {
        _questionOptionRepository = questionOptionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeleteQuestionOptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting DeleteQuestionOption: Deleting question option {OptionId}", request.Id);

        // Verify question option exists
        if (!await _questionOptionRepository.ExistsAsync(request.Id, cancellationToken))
        {
            _logger.LogWarning("Question option not found: {OptionId}", request.Id);
            throw new EntityNotFoundException(nameof(QuestionOption), request.Id);
        }

        // Delete question option
        await _questionOptionRepository.DeleteAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Success: Successfully deleted question option {OptionId}", request.Id);
    }
}

/// <summary>
/// Handler for getting all question options
/// </summary>
public class GetAllQuestionOptionsQueryHandler : IQueryHandler<GetAllQuestionOptionsQuery, GetQuestionOptionsResponse>
{
    private readonly IQuestionOptionRepository _questionOptionRepository;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public GetAllQuestionOptionsQueryHandler(
        IQuestionOptionRepository questionOptionRepository,
        IMapper mapper,
        ILoggingService logger)
    {
        _questionOptionRepository = questionOptionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetQuestionOptionsResponse> Handle(GetAllQuestionOptionsQuery request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("GetAllQuestionOptions");

        try
        {
            _logger.LogOperationStart<QuestionOption>(logContext, "Retrieving all question options");

            var questionOptions = await _questionOptionRepository.GetAllAsync(cancellationToken);
            var questionOptionDtos = _mapper.Map<IEnumerable<GetQuestionOptionResponse>>(questionOptions);

            _logger.LogOperationSuccess<QuestionOption>(logContext, "Successfully retrieved {Count} question options", questionOptions.Count());

            return new GetQuestionOptionsResponse
            {
                Options = questionOptionDtos,
                TotalCount = questionOptions.Count()
            };
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<QuestionOption>(logContext, ex, "Error occurred while retrieving all question options");
            throw;
        }
    }
}

/// <summary>
/// Handler for getting a question option by ID
/// </summary>
public class GetQuestionOptionByIdQueryHandler : IQueryHandler<GetQuestionOptionByIdQuery, GetQuestionOptionResponse?>
{
    private readonly IQuestionOptionRepository _questionOptionRepository;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public GetQuestionOptionByIdQueryHandler(
        IQuestionOptionRepository questionOptionRepository,
        IMapper mapper,
        ILoggingService logger)
    {
        _questionOptionRepository = questionOptionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetQuestionOptionResponse?> Handle(GetQuestionOptionByIdQuery request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("GetQuestionOptionById");

        try
        {
            _logger.LogOperationStart<QuestionOption>(logContext, "Retrieving question option with ID: {OptionId}", request.Id);

            var questionOption = await _questionOptionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (questionOption == null)
            {
                _logger.LogOperationWarning<QuestionOption>(logContext, "Question option with ID {OptionId} not found", request.Id);
                return null;
            }

            _logger.LogOperationSuccess<QuestionOption>(logContext, "Successfully retrieved question option with ID: {OptionId}", request.Id);

            return _mapper.Map<GetQuestionOptionResponse>(questionOption);
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<QuestionOption>(logContext, ex, "Error occurred while retrieving question option with ID: {OptionId}", request.Id);
            throw;
        }
    }
}

/// <summary>
/// Handler for getting question options by question ID
/// </summary>
public class GetQuestionOptionsByQuestionIdQueryHandler : IQueryHandler<GetQuestionOptionsByQuestionIdQuery, GetQuestionOptionsResponse>
{
    private readonly IQuestionOptionRepository _questionOptionRepository;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public GetQuestionOptionsByQuestionIdQueryHandler(
        IQuestionOptionRepository questionOptionRepository,
        IMapper mapper,
        ILoggingService logger)
    {
        _questionOptionRepository = questionOptionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetQuestionOptionsResponse> Handle(GetQuestionOptionsByQuestionIdQuery request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("GetQuestionOptionsByQuestionId");

        try
        {
            _logger.LogOperationStart<QuestionOption>(logContext, "Retrieving question options for question ID: {QuestionId}", request.QuestionId);

            var questionOptions = await _questionOptionRepository.GetByQuestionIdAsync(request.QuestionId, cancellationToken);
            var questionOptionDtos = _mapper.Map<IEnumerable<GetQuestionOptionResponse>>(questionOptions);

            _logger.LogOperationSuccess<QuestionOption>(logContext, "Successfully retrieved {Count} question options for question ID: {QuestionId}",
                questionOptions.Count(), request.QuestionId);

            return new GetQuestionOptionsResponse
            {
                Options = questionOptionDtos,
                TotalCount = questionOptions.Count()
            };
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<QuestionOption>(logContext, ex, "Error occurred while retrieving question options for question ID: {QuestionId}", request.QuestionId);
            throw;
        }
    }
}

/// <summary>
/// Handler for getting correct options by question ID
/// </summary>
public class GetCorrectOptionsByQuestionIdQueryHandler : IQueryHandler<GetCorrectOptionsByQuestionIdQuery, GetQuestionOptionsResponse>
{
    private readonly IQuestionOptionRepository _questionOptionRepository;
    private readonly IMapper _mapper;
    private readonly ILoggingService _logger;

    public GetCorrectOptionsByQuestionIdQueryHandler(
        IQuestionOptionRepository questionOptionRepository,
        IMapper mapper,
        ILoggingService logger)
    {
        _questionOptionRepository = questionOptionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetQuestionOptionsResponse> Handle(GetCorrectOptionsByQuestionIdQuery request, CancellationToken cancellationToken)
    {
        var logContext = LogContext.Create("GetCorrectOptionsByQuestionId");

        try
        {
            _logger.LogOperationStart<QuestionOption>(logContext, "Retrieving correct options for question ID: {QuestionId}", request.QuestionId);

            var questionOptions = await _questionOptionRepository.GetCorrectOptionsByQuestionIdAsync(request.QuestionId, cancellationToken);
            var questionOptionDtos = _mapper.Map<IEnumerable<GetQuestionOptionResponse>>(questionOptions);

            _logger.LogOperationSuccess<QuestionOption>(logContext, "Successfully retrieved {Count} correct options for question ID: {QuestionId}",
                questionOptions.Count(), request.QuestionId);

            return new GetQuestionOptionsResponse
            {
                Options = questionOptionDtos,
                TotalCount = questionOptions.Count()
            };
        }
        catch (Exception ex)
        {
            _logger.LogOperationError<QuestionOption>(logContext, ex, "Error occurred while retrieving correct options for question ID: {QuestionId}", request.QuestionId);
            throw;
        }
    }
}
