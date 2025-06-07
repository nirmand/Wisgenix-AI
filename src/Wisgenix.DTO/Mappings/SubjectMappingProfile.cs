using AutoMapper;
using Wisgenix.Data.Entities;
using Wisgenix.DTO;

namespace Wisgenix.DTO.Mappings;

public class SubjectMappingProfile : Profile
{
    public SubjectMappingProfile()
    {
        CreateMap<Subject, SubjectDto>()
            .ForMember(dest => dest.Topics, opt => opt.MapFrom(src => src.Topics ?? new List<Topic>()));
            
        CreateMap<CreateSubjectDto, Subject>();
        
        CreateMap<UpdateSubjectDto, Subject>()
            .ForMember(dest => dest.ID, opt => opt.Ignore());
    }
}