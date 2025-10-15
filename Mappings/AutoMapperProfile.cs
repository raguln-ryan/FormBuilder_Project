using AutoMapper;
using FormBuilder.API.DTOs.Auth;
using FormBuilder.API.DTOs.Form;
using FormBuilder.API.Models;

namespace FormBuilder.API.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<User, AuthResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

            // Form mappings - Updated to use new DTOs
            CreateMap<FormLayoutRequestDto, Form>();
            CreateMap<Form, FormLayoutRequestDto>();
            
            CreateMap<FormLayoutResponseDto, Form>();
            CreateMap<Form, FormLayoutResponseDto>();
            
            CreateMap<FormConfigRequestDto, Form>();
            CreateMap<Form, FormConfigResponseDto>();

            // Question mappings
            CreateMap<Question, Question>().ReverseMap();
        }
    }
}
