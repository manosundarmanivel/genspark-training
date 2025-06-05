using AutoMapper;
using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;

namespace FirstAPI.Misc
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<DoctorAddDto, User>()
            .ForMember(dest => dest.Username, act => act.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<User, DoctorAddDto>()
            .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Username));
            
            CreateMap<PatientAddDto, User>()
            .ForMember(dest => dest.Username, act => act.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.Ignore());
           
            CreateMap<User, PatientAddDto>()
            .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Username));
   
        }
    }
}