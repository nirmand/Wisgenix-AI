using AutoMapper;
using Wisgenix.Data.Entities;
using Wisgenix.DTO;

namespace Wisgenix.DTO.Mappings;

public class TopicMappingProfile : Profile
{
    public TopicMappingProfile()
    {
        CreateMap<Topic, TopicDto>()
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectName : String.Empty))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy));
            
        CreateMap<CreateTopicDto, Topic>();
        
        CreateMap<UpdateTopicDto, Topic>()
            .ForMember(dest => dest.ID, opt => opt.Ignore());
    }
}