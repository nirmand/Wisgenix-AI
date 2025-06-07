using AutoMapper;
using Wisgenix.Data.Entities;
using Wisgenix.DTO;

namespace Wisgenix.DTO.Mappings;

public class QuestionOptionMappingProfile : Profile
{
    public QuestionOptionMappingProfile()
    {
        CreateMap<QuestionOption, QuestionOptionDto>()
            .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question != null ? src.Question.QuestionText : string.Empty));
            
        CreateMap<CreateQuestionOptionDto, QuestionOption>();
        
        CreateMap<UpdateQuestionOptionDto, QuestionOption>()
            .ForMember(dest => dest.ID, opt => opt.Ignore());
    }
}