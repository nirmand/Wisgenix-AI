using AutoMapper;
using Content.Application.DTOs;
using Content.Domain.Entities;

namespace Content.Application.Mappings;

/// <summary>
/// AutoMapper profile for Content bounded context
/// </summary>
public class ContentMappingProfile : Profile
{
    public ContentMappingProfile()
    {
        // Subject mappings
        CreateMap<Subject, GetSubjectResponse>()
            .ForMember(dest => dest.Topics, opt => opt.MapFrom(src => src.Topics ?? new List<Topic>()));

        CreateMap<AddSubjectRequest, Subject>()
            .ConstructUsing(src => new Subject(src.SubjectName));

        // Topic mappings
        CreateMap<Topic, GetTopicResponse>()
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectName : string.Empty))
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions ?? new List<Question>()));

        CreateMap<AddTopicRequest, Topic>()
            .ConstructUsing(src => new Topic(src.TopicName, src.SubjectId));

        // Question mappings
        CreateMap<Question, GetQuestionResponse>()
            .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic != null ? src.Topic.TopicName : string.Empty))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options ?? new List<QuestionOption>()));

        CreateMap<AddQuestionRequest, Question>()
            .ConstructUsing(src => new Question(src.QuestionText, src.TopicId, src.DifficultyLevel, src.MaxScore, src.GeneratedBy, src.QuestionSourceReference));

        // QuestionOption mappings
        CreateMap<QuestionOption, GetQuestionOptionResponse>();

        CreateMap<AddQuestionOptionRequest, QuestionOption>()
            .ConstructUsing(src => new QuestionOption(src.OptionText, src.QuestionId, src.IsCorrect));
    }
}
