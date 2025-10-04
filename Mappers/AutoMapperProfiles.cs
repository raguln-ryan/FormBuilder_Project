using AutoMapper;
using FormBuilder.API.Models.Relational;
using FormBuilder.API.Models.Document;
using FormBuilder.API.DTOs;
using FormBuilder.API.BusinessLayer.DTOs;

namespace FormBuilder.API.Mappers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Relational Entities
            CreateMap<FormMetadata, FormMetadataDTO>().ReverseMap();

            // Document Entities
            CreateMap<Section, SectionDTO>().ReverseMap();
            CreateMap<Field, FieldDTO>().ReverseMap();
            CreateMap<Response, ResponseDTO>().ReverseMap();

            

            // **Add this mapping for submission**
            CreateMap<FormSubmissionDTO, Response>()
    .ForMember(dest => dest.SubmittedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
    .ForMember(dest => dest.Id, opt => opt.Ignore());

        }
    }
}
