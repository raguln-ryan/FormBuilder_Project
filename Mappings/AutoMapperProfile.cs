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

            // Form mappings
            CreateMap<FormLayoutDto, Form>();
            CreateMap<Form, FormLayoutDto>();

            // Question mappings
            CreateMap<Question, Question>().ReverseMap();
        }
    }
}
