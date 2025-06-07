using AutoMapper;
using Wisgenix.Data.Entities;
using Wisgenix.DTO;

namespace Wisgenix.DTO.Mappings;

public class QuestionMappingProfile: Profile
{
 public QuestionMappingProfile()
    {
        CreateMap<Question, QuestionDto>()
            .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic != null ? src.Topic.TopicName : string.Empty));
            
        CreateMap<CreateQuestionDto, Question>();
        
        CreateMap<UpdateQuestionDto, Question>()
            .ForMember(dest => dest.ID, opt => opt.Ignore());
    }
}
