using AutoMapper;
using AIUpskillingPlatform.Data.Entities;
using AIUpskillingPlatform.DTO;

namespace AIUpskillingPlatform.DTO.Mappings;

public class TopicMappingProfile : Profile
{
    public TopicMappingProfile()
    {
        CreateMap<Topic, TopicDto>()
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectName : String.Empty));
            
        CreateMap<CreateTopicDto, Topic>();
        
        CreateMap<UpdateTopicDto, Topic>()
            .ForMember(dest => dest.ID, opt => opt.Ignore());
    }
}